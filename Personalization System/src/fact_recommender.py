import numpy as np
import random
from sklearn.neighbors import NearestNeighbors
from sklearn.preprocessing import MinMaxScaler
from src.aws import Aws


# called by app.py
def get_personalized_fact_ids(user_id, poi_id, num_facts):
    if num_facts <= 0:
        return []

    aws = Aws()
    poi_facts = aws.load_all_poi_facts(poi_id)
    user_item = aws.load_user_item(user_id)

    user_vector = get_category_vector(user_item['FieldsOfInterest'])

    non_zero_score_facts = get_facts_with_non_zero_scores(user_vector, poi_facts)
    #print("Non zero score facts:\n" + str(non_zero_score_facts))

    # if we have enough facts...
    if len(non_zero_score_facts) >= num_facts:
        fact_ids = choose_fact_ids(non_zero_score_facts, num_facts)
        random.shuffle(fact_ids)
        return fact_ids

    # otherwise use KNN method to get remaining facts...
    else:
        num_facts_left = num_facts - len(non_zero_score_facts)
        avg_user_vector = get_avg_preferences_of_similar_users(user_item)
        #print("Average category vector:\n" + str(avg_user_vector))

        # list of current IDs of non-zero score fact
        fact_ids = [x[0] for x in non_zero_score_facts]

        # Remove current non-zero facts from poi_facts
        poi_facts = [fact for fact in poi_facts if not fact['FactId'] in fact_ids]

        # get remaining facts using average preferences of similar users
        new_facts = get_facts_with_non_zero_scores(avg_user_vector, poi_facts)
        fact_ids += choose_fact_ids(new_facts, num_facts_left)

        # if we got enough facts using KNN search...
        if len(fact_ids) >= num_facts:
            return fact_ids

        # otherwise if we still do not have enough facts, just randomly choose the remaining facts...
        else:
            remaining_fact_ids = [int(fact['FactId']) for fact in poi_facts if not fact['FactId'] in fact_ids]
            fact_ids += random.sample(remaining_fact_ids, num_facts - len(fact_ids))
            return fact_ids


# called by app.py
# returns {num_facts} fact IDs with at least 1 ID of a miscellaneous fact
def get_random_fact_ids(poi_id, num_facts):
    if num_facts <= 0:
        return []

    aws = Aws()
    poi_facts = aws.load_all_poi_facts(poi_id)

    all_fact_ids = [int(fact['FactId']) for fact in poi_facts]

    # IDs of all miscellaneous facts
    all_misc_ids = [int(fact['FactId']) for fact in poi_facts if int(fact['Categories']['Miscellaneous']) > 0]

    # choose one misc. fact ID and remove it from all_fact_ids
    misc_id = random.choice(all_misc_ids)
    all_fact_ids.remove(misc_id)

    # random fact IDs with at least 1 ID of a miscellaneous fact
    random_ids = [misc_id] + random.sample(all_fact_ids, num_facts - 1)

    random.shuffle(random_ids)

    return random_ids


# called by app.py
# returns a list of dictionaries where each dict. contains the a fact and all categories it belongs to
def get_facts(poi_id, fact_ids):
    facts = []

    aws = Aws()
    for fact_id in fact_ids:
        fact_item = aws.load_poi_fact(poi_id, fact_id)

        # only add categories that actually belong to the fact
        categories = [key for key in fact_item['Categories'].keys() if fact_item['Categories'][key] > 0]

        facts.append({
            'fact': fact_item['Fact'],
            'categories': categories
        })

    return facts


# returns a list of (fact_id, score) tuples where score > 0
# returned tuples are sorted by score in descending order
def get_facts_with_non_zero_scores(user_vector, poi_facts):
    if len(poi_facts) == 0:
        return []

    non_zero_score_facts = []

    for fact in poi_facts:
        fact_id = int(fact['FactId'])
        fact_vector = get_category_vector(fact['Categories'])

        score = np.dot(user_vector, fact_vector)

        if score > 0:
            non_zero_score_facts.append((fact_id, score))

    return sorted(non_zero_score_facts, key=lambda x: x[1], reverse=True)


# input parameter {facts} is a list of (fact_id, score) tuples
# function chooses and returns {min(num_facts, len(facts))} fact IDs starting with the fact with highest score
# if multiple facts have the same score, choose randomly
# example: num_facts=2 and there is one fact with score 4, two facts with score 3 and two facts with score 2
#          ==> choose fact with score 4 and randomly choose one of the two facts with score 3
def choose_fact_ids(facts, num_facts):
    if len(facts) == 0:
        return []

    fact_ids = []
    num_facts_left = min(num_facts, len(facts))
    current_score = facts[0][1]     # initialize current_score with highest score
    current_ids = []    # list of all IDs of facts where score == current_score

    for f in facts:
        if f[1] == current_score:
            current_ids.append(f[0])
        else:
            if len(current_ids) >= num_facts_left:
                fact_ids += random.sample(current_ids, num_facts_left)
                return fact_ids
            else:
                fact_ids += random.sample(current_ids, len(current_ids))

                num_facts_left -= len(current_ids)
                current_score = f[1]
                current_ids = [f[0]]

    fact_ids += random.sample(current_ids, num_facts_left)

    return fact_ids


# uses KNN to find similar users and returns the average preferences of them
def get_avg_preferences_of_similar_users(user_item, num_similar_users=3):
    aws = Aws()
    other_user_items = aws.load_all_other_user_items(user_item['UserId'])

    user_features = np.array(get_knn_features(user_item))

    # dictionary which maps row IDs to User IDs
    row_id_user_id_dict = {}

    # create feature matrix
    other_user_features = []
    for i in range(len(other_user_items)):
        features = get_knn_features(other_user_items[i])
        other_user_features.append(features)
        row_id_user_id_dict[i] = other_user_items[i]['UserId']

    other_user_features = np.array(other_user_features)

    # scale features
    scaler = MinMaxScaler().fit(other_user_features)
    other_user_features = scaler.transform(other_user_features)
    user_features = scaler.transform([user_features])[0] # transform function needs 2D array...

    # use KNN to get indices of similar rows
    knn = NearestNeighbors(n_neighbors=num_similar_users).fit(other_user_features)
    distances, indices = knn.kneighbors([user_features])

    # use mapping from row IDs to User IDs to get IDs of similar users
    similar_user_ids = [row_id_user_id_dict[index] for index in indices[0]]

    # get category vector of each similar user
    similar_user_items = [item for item in other_user_items if item['UserId'] in similar_user_ids]
    category_vectors = np.array([get_category_vector(item['FieldsOfInterest']) for item in similar_user_items])

    # return average of all category vectors
    return category_vectors.mean(axis=0)


# works for both users and facts, vectors are used to calculate scores
def get_category_vector(categories):
    vector = [int(categories['Politics']),
              int(categories['History']),
              int(categories['Economy']),
              int(categories['Art']),
              int(categories['Religion']),
              int(categories['Technology'])]

    return np.array(vector)


# returns vector of user features needed for the KNN search
def get_knn_features(user_item):
    traits = user_item['PersonalityTraits']

    features = [int(user_item['Age']),
                1 if user_item['Gender'] == 'Male' else 0,
                int(traits['Open']),
                int(traits['Dependable']),
                int(traits['Extraverted']),
                int(traits['Agreeable']),
                int(traits['Calm'])]

    return features
