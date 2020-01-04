import plotly.graph_objects as plot
import datetime
import argparse

parser=argparse.ArgumentParser()
parser.add_argument("integers", metavar="x", nargs="+")
parser.add_argument("integers", metavar="y", nargs="+")

args=parser.parse_args()
x=args.x.split(',')
y=args.y.split(',')

#print("x:\n")
#x=input().split(',')
#print("y:\n")
#y=input().split(',')

now=datetime.datetime.now()
filename=now.strftime("%Y-%m-%d %H:%M:%S.svg")

fig=plot.Figure(data=plot.Scatter(x=x,y=y,mode="markers"))
fig.write_image(filename)

print(filename)
