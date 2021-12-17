from flask import Flask, request, jsonify
import src.fact_recommender as fact_recommender

app = Flask(__name__)


@app.route('/')
def hello():
    print("hello")
    return 'hello there'


@app.route('/poi-facts-recommendation-query', methods=['POST'])
def get_facts():
    print("received request")

    recommendation = {'items': []}
    try:
        request_data = request.get_json()

        user_id = int(request_data['userId'])
        poi_id = int(request_data['poiId'])
        num_facts = int(request_data['numFacts'])
        retrieve_personalized_facts = request_data['retrievePersonalizedFacts']

        if retrieve_personalized_facts:
            print("retrieving personalized facts...")
            fact_ids = fact_recommender.get_personalized_fact_ids(user_id=user_id, poi_id=poi_id, num_facts=num_facts)
        else:
            print("retrieving random facts...")
            fact_ids = fact_recommender.get_random_fact_ids(poi_id=poi_id, num_facts=num_facts)

        print('Fact IDs: ' + str(fact_ids))

        recommendation['items'] = fact_recommender.get_facts(poi_id=poi_id, fact_ids=fact_ids)

        return jsonify(recommendation)

    except Exception as e:
        print(e)
        return recommendation


if __name__ == '__main__':
    app.run()
