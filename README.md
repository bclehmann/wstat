
# Statistics-CLI

A *currently* very basic CLI for some stats stuff. It can list and it can summarize a numerical dataset.

    stat set=1,2,3 list

Will print something like this

    
	1.000000000
    2.000000000
    3.000000000

You can also do `stat set=1,2,3 operation=list` if you're not a fan of the shorthand. The command line arguments can go in any order. For example `stat list set=1,2,3` is equally valid.

## Summarizing a Dataset

Lists are cool and all, but at the end of the day, what you care about is probably more down here. `stat set=1,2,3 summary` will print something like this:

	Min             Q1              Med             Q3              Max
	1.000000000     1.000000000     2.000000000     3.000000000     3.000000000

    Mean            Std. Dev. (s)   Std. Dev. (σ)
    2.000000000     1.000000000     0.816496581

## Using stdin and the Pipe Operator

It's not terribly convenient to pass in everything as a command line argument, so you can pass in your set into stdin. This means that if `rng` is a command that creates a comma-separated list of numbers, `rng | stat summary` will let you summarize that list easily.

In addition, you can also give no input and no command-line parameters: `stat summary` and the console will let you write your input, which will be read when you hit enter. This is useful when you are copy-pasting your dataset from elsewhere.

## Open Source

This project is a very low priority for me, so if you decide to open some pull-requests to add features or otherwise improve code quality I would be very thankful.

---
![Logo](/images/logo_full.png)

# Made by Where 1
