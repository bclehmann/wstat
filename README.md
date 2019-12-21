

# Statistics-CLI

A *currently* very basic CLI for some stats stuff. It can list and it can summarize a numerical dataset. For 2D data it may also plot it.

    wstat set=1,2,3 list

Will print something like this

    
	1.000000000
    2.000000000
    3.000000000

You can also do `wstat set=1,2,3 operation=list` if you're not a fan of the shorthand. The command line arguments can go in any order. For example `wstat list set=1,2,3` is equally valid. In addition, `list` is the default opperation, so `wstat set=1,2,3` is enough. If you specify your set in one of the additional ways below, you can get away with just `wstat`

## Summarizing a Dataset

Lists are cool and all, but at the end of the day, what you care about is probably more down here. `wstat set=1,2,3 summary` will print something like this:

	Min             Q1              Med             Q3              Max
	1.000000000     1.000000000     2.000000000     3.000000000     3.000000000

    Mean            Std. Dev. (s)   Std. Dev. (σ)
    2.000000000     1.000000000     0.816496581
    
## Outlier Detection

You can also use this `summary` command to indicate potential outliers. They will be right under your normal summary.

## Correlation

Type `correlate` or `correlation` to print the `r` correlation coefficient

## Using stdin and the Pipe Operator

It's not terribly convenient to pass in everything as a command line argument, so you can pass in your set into stdin. This means that if `rng` is a command that creates a comma-separated list of numbers, `rng | wstat summary` will let you summarize that list easily.

In addition, you can also give no input and no command-line parameters: `wstat summary` and the console will let you write your input, which will be read when you hit enter. This is useful when you are copy-pasting your dataset from elsewhere.

## Output (text, json or csv)

Currently json, csv and text output are supported. Text is the default. You can specify with `wstat list json` or `wstat list text`. The full version is `wstat operation=list output=json`. The same information is given in both the list and the summary, it's just displayed differently. Please note that the only operation that supports CSV output is List.

## Read/Write from file

You can specify an output file with `wstat -o C:\Users\benny\Desktop\a.txt` or `wstat file=C:\Users\benny\Desktop\a.txt` The directory you point to must exist. You can read a set from an input file with `wstat set=C:\Users\benny\Desktop\a.txt` or specify the file with stdin.

When a file path is specified as a commandline argument, if it has spaces, it must be in quotes.

## Multivariable Sets

This supports n-dimensions, just add `dimensions=2` to your command. A shorthand in the case of 2-D data, is `2var`. If you have 3+ dimensional data, you need to use `dimensions=3`

## Plotting 2-D Sets

Pretty simple, just do `wstat 2var plot` and once you type in your data it will give you the filepath.

For example, a set like this: `(-3,9),(-2,4),(-1,1),(0,0),(1,1),(2,4),(3,9)`

Would produce a plot like this:

![Plot](/demo/parabola.bmp)

The parantheses around points are optional, I include them because I prefer the notation, especially for a demo/tutorial.

## Linear Regression

Add a `linreg` to your plot command `wstat 2var plot linreg` and it will give you a printout as well as draw the Least Square Regression Line on the graph. `linreg` is a shorthand for `options=linreg`, if you use multiple options you can do `options=option1,linreg,option3` or you can use the shorthands together.

For the set `-2,2,2,0.11134,0,0.8` I got this plot:
![linreg Plot](/demo/linreg.bmp)

And this printout: 
	
	    y=a+bx
		
        a=0.9704466666666667
        b=-0.472165


        Filepath: C:\Users\benny\source\repos\stat\statistics-cli/plots/plot_2019-10-28___08-35-24_PM.bmp
		
## Reexpression

Add a `reexpress`, and then choose either `zscore` or `residual` as an option. `zscore` works on N-Dimensional sets, `residual` requires a 2-Dimensional set. `residual` also requires you specify the regression line to use.

`wstat reexpress zscore` will standardize a 1-Dimensional set.

`wstat reexpress zscore 2var` for 2 dimensions.

`wstat reexpress zscore dimensions=<n>` for n dimensions.

`wstat reexpress residual 2var linreg` will print the residual set for a linear regression line in two dimensions.

## Normal distribution

Type `wstat cdf` for the cumulative distribution function for a normal distribution. Type `wstat invCdf` for the inverse.

## Linux Users

This does work on Linux, all you have to do is find the file `wstat` in `bin/Release/netcoreapp3.0/linux-x64`, make it executable, and then run. If you need help making it executable, this should do: https://askubuntu.com/questions/229589/how-to-make-a-file-e-g-a-sh-script-executable-so-it-can-be-run-from-a-termi

Please make sure you do this to the file `wstat` with no extension, not `wstat.dll` or anything like that.

For drawing plots you will need two packages: `libc6-dev` and `libgdiplus` For some distributions, `xdg-utils` may not come preinstalled, and that is needed for auto-opening images.


For APT

`sudo apt install libc6-dev`

`sudo apt install libgdiplus`

xdg-utils (Probably already installed)

`sudo apt install xdg-utils`

For yum

`sudo yum install glibc-devel`

`sudo yum install glibc-devel.i686`

`sudo yum install libgdiplus`

xdg-utils (Probably already installed)

`sudo yum install xdg-utils`

## OSX Users

It should work, but I have no idea how well it works, I don't have a Mac to test it on, and I am hesitant to pirate OSX and stick it in a VM. Let me know how it goes if you try it.

## 32 Bit, ARM, etc

I have only built x86-64 binaries, but as it is open source, anything that .NET Core will compile on should work. The reason for this, is that I cannot test on a processor that I do not have.

Keep in mind, required packages may differ if you run on 32-bit x86 or on ARM.

## Open Source

This project is a very low priority for me, so if you decide to open some pull-requests to add features or otherwise improve code quality I would be very thankful.

Please note, I would like to have CI sorted out on PRs, however github actions is not currently cooperating with .NET Core 3.0, so we are currently stuck without it.

In addition, everything is subject to code-review

---
![Logo](/images/logo_full.png)

# Made by Where 1
