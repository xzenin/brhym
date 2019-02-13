from utils import generate_random_start, generate_from_seed
from keras.models import load_model
import tensorflow as tf

from flask import Flask, request, jsonify
import sys, os,  inspect
from pathlib import Path
from flask_cors import CORS, cross_origin

 # realpath() will make your script run, even if you symlink it :)
cmd_folder = os.path.realpath(os.path.abspath(os.path.split(inspect.getfile( inspect.currentframe() ))[0]))
if cmd_folder not in sys.path:
    sys.path.insert(0, cmd_folder)
'''
 # Use this if you want to include modules from a subfolder
cmd_subfolder = os.path.realpath(os.path.abspath(os.path.join(os.path.split(inspect.getfile( inspect.currentframe() ))[0],"BanglaModel")))
if cmd_subfolder not in sys.path:
    sys.path.insert(0, cmd_subfolder)
'''
from BanglaModel import BanglaModel, ModelReader

print("Current Folder: " , cmd_folder)
#need fix
datafolder = cmd_folder.replace("web", "")

# Create app
app = Flask(__name__)
app.config['CORS_HEADERS'] = 'Content-Type'
CORS(app, resources={r"/*": {"origins": "*"}})

loaded = False

def load_bangla_model():
    """Load in the pre-trained model"""
    global model 
    global reader 
    global loaded 
    datafile =  datafolder + "sunil.mdl"
    reader = ModelReader(datafile)
    model = reader.read()
    loaded = True
    print("loaded, trying one")
    print(reader.nextline("চলে", 5))

    
# Home page
@app.route("/api/v1", methods=['GET', 'POST'])
def home():
    """Home page of app with form"""
    return "<h1>Not Much Going On Here</h1>"

# Home page
@app.route("/api/v1/suggest", methods=['POST', 'GET', 'OPTIONS'])
@cross_origin(origin='localhost')
def suggest():

    if(loaded == False):
        load_bangla_model()
        
    if(request.method == "POST"):
        json = request.get_json()
        args = json["query"]
        print(args["seed"])
        sentences = []
        sentence = reader.nextline(args["seed"], int( args["diversity"] ))
        print(sentence)
        sentences.append(sentence)
        #words = args["words"].split(" ")
        #for word in words:
        #    sentence = reader.nextline(word, args.diversity)
        return jsonify(sentences)
    else:
        return jsonify("{result:'Expecting a POST Call'}")


if __name__ == "__main__":
    print(("* Loading Keras model and Flask starting server..."
           "please wait until server has fully started"))
    load_bangla_model()
    # Run app
    app.run(host="127.0.0.1", port=8098)
