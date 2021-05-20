import requests as rq
import pandas as pd
import json
import pickle
import numpy as np
import json

class PlayerProb(object):
    def __init__(self, name, awper, entryFragger, lurker, suporte):
        self._name = name
        self._awper = awper
        self._entryFragger = entryFragger
        self._lurker = lurker
        self._suporte = suporte

    def to_dict(self):
        return {"Name": self._name, "AWPer": self._awper, "Entry Fragger": self._entryFragger, "Lurker": self._lurker, "Suporte": self._suporte}

    @property
    def name(self):
        return self._name

    @property
    def awper(self):
        return self._awper

    @awper.setter
    def awper(self, value):
        self._awper = value

    @property
    def entryFragger(self):
        return self._entryFragger
    
    @entryFragger.setter
    def entryFragger(self, value):
        self._entryFragger = value

    @property
    def lurker(self):
        return self._lurker

    @lurker.setter
    def lurker(self, value):
        self._lurker = value

    @property
    def suporte(self):
        return self._suporte

    @suporte.setter
    def suporte(self, value):
        self._suporte = value
        
url_api = "https://localhost:44346/v1/Demo/"

jsonResult = rq.get(url_api + "GetPlayers", verify=False).content
players_stats = pd.read_json(jsonResult)
players_stats.drop(['Weapons'], axis=1, inplace=True)

predict_df = players_stats.drop(['Name', 'TeamName', 'WalkQuantityAsCT', 'DistanceTraveledAsCT'], axis=1)
predict_df.insert(0, 'Role', '')
predict_df = predict_df.fillna(0)
predict_df = predict_df.round(decimals=2)

with open('models/model62.pkl', 'rb') as file:
    loaded_model = pickle.load(file)

predictions_proba = loaded_model.predict_proba(predict_df.drop('Role', axis=1))
predictions = loaded_model.predict(predict_df.drop('Role', axis=1))
predictions_proba = np.round(predictions_proba, decimals=2)

players_proba = pd.DataFrame(predictions_proba)
players_proba.columns = ['AWPer', 'Entry Fragger', 'Lurker', 'Suporte']
players_proba.insert(0, 'Name', '')
players_proba['Name'] = players_stats['Name']
#print(players_proba)

playersList = []
rows = len(players_proba.index)
cols = len(players_proba.columns)
for i in range(rows):
    playersList.append(PlayerProb(players_proba.iloc[i][0], players_proba.iloc[i][1], players_proba.iloc[i][2], players_proba.iloc[i][3], players_proba.iloc[i][4]))

results = [player.to_dict() for player in playersList]
playersjson = json.dumps(results)
json_file = open('json/PlayersRole.json', 'w')
json_file.write(playersjson)
json_file.close()

