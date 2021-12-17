import boto3
from boto3.dynamodb.conditions import Attr


class Aws:
    def __init__(self):
        session = boto3.Session(
            aws_access_key_id='',
            aws_secret_access_key='',
            region_name='eu-central-1'
        )

        self.dynamodb = session.resource('dynamodb')

    def load_user_item(self, user_id):
        table = self.dynamodb.Table('users')

        response = table.get_item(
            Key={
                'UserId': user_id
            }
        )

        return response['Item']

    def load_all_other_user_items(self, user_id):
        table = self.dynamodb.Table('users')

        response = table.scan()
        items = response['Items']

        # remove our current user from list of all other users
        for item in items:
            if item['UserId'] == user_id:
                items.remove(item)
                break

        return items

    def load_all_poi_facts(self, poi_id):
        table = self.dynamodb.Table('poi-facts')

        response = table.scan(
            FilterExpression=Attr('PoiId').eq(poi_id),
            ProjectionExpression='FactId, Categories'
        )

        return response['Items']

    def load_poi_fact(self, poi_id, fact_id):
        table = self.dynamodb.Table('poi-facts')

        response = table.get_item(
            Key={
                'PoiId': poi_id,
                'FactId': fact_id
            },
            ProjectionExpression='Fact, Categories'
        )

        return response['Item']