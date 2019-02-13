from flask import Flask, request, jsonify
import sys, os,  inspect
from pathlib import Path
from flask_cors import CORS, cross_origin

 # realpath() will make your script run, even if you symlink it :)
cmd_folder = os.path.realpath(os.path.abspath(os.path.split(inspect.getfile( inspect.currentframe() ))[0]))
#need fix
datafolder = cmd_folder.replace("web", "")

# Create app
app = Flask(__name__)
app.config['CORS_HEADERS'] = 'Content-Type'
CORS(app, resources={r"/*": {"origins": "*"}})

# Home page
@app.route("/api/v1", methods=['GET', 'POST'])
def home():
    """Home page of app with form"""
    return "<h1>Not Much Going On Here</h1>"

# Home page
@app.route("/api/v1/suggest", methods=['POST', 'GET', 'OPTIONS'])
@cross_origin(origin='localhost')
def suggest():
    if(request.method == "POST"):
        args = request.get_json()
        print(args.query.seed)
        return jsonify(args)
    else:
        return jsonify("{result:'Expecting a POST Call'}")
if __name__ == "__main__":
    print(("* Loading Keras model and Flask starting server..."
           "please wait until server has fully started"))
    # Run app
    app.run() 
    #host="127.0.0.1", port=8098)
