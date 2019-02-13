
from flask import Flask, render_template, request
from wtforms import Form, TextField, validators, SubmitField, DecimalField, IntegerField
import sys, os,  inspect
from pathlib import Path



# Create app
app = Flask(__name__)


class ReusableForm(Form):
    """User entry form for entering specifics for generation"""
    # Starting seed
    seed = TextField("বাংলা লেখার জন্যঃ :", validators=[
                     validators.InputRequired()])
    # Diversity of predictions
    diversity = DecimalField('Enter diversity:', default=0.8,
                             validators=[validators.InputRequired(),
                                         validators.NumberRange(min=0.5, max=5.0,
                                                                message='ডাইভারসিটি ০৫ থেকে ৫ ')])
    # Number of words
    words = IntegerField('যতগুলো শব্দ তৈরি করবেন :',
                         default=50, validators=[validators.InputRequired(),
                                                 validators.NumberRange(min=10, max=100, message='এক থেকে কুড়িটা শব্দ নির্মান করা যাবে')])
    # Submit button
    submit = SubmitField("যান")


@app.route("/help", methods=['GET', 'POST'])
def help():
    """Home page of app with form"""
    # Create form
    form = ReusableForm(request.form)

    # Send template information to index.html
    return render_template('help.html', form=form)


# Home page
@app.route("/", methods=['GET', 'POST'])
def home():
    """Home page of app with form"""
    # Create form
    form = ReusableForm(request.form)

    # On form entry and all conditions met
    if request.method == 'POST' and form.validate():
        # Extract information
        seed = request.form['seed']
        diversity = float(request.form['diversity'])
        words = int(request.form['words'])
        # Generate a random sequence
    # Send template information to index.html
    return render_template('index.html', form=form)


if __name__ == "__main__":
    print(("* Loading Keras model and Flask starting server..."
           "please wait until server has fully started"))
    # Run app
    app.run(host="0.0.0.0", port=8082)
