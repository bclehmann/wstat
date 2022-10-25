# THIS PROJECT IS VULNERABLE TO [CVE-2021-24112](https://github.com/advisories/GHSA-rxg9-xrhp-64gj) ON LINUX AND MAC-OS. THIS PROJECT IS NOT BEING MAINTAINED, USE AT YOUR OWN RISK
This vulnerability is labeled Critical with a CVSS score of 9.8/10



# Statistics-CLI

A *currently* very basic CLI for some stats stuff. It can list and it can summarize a numerical dataset. For 2D data it may also plot it. Keep in mind, that regardless of the number of dimensions, the last item in an ordered pair is taken to be the coordinate of the **RESPONDING** variable.

    wstat set=1,2,3 list

Will print something like this

    
	1.000000000
    2.000000000
    3.000000000

You can also do `wstat set=1,2,3 operation=list` if you're not a fan of the shorthand. The command line arguments can go in any order. For example `wstat list set=1,2,3` is equally valid. In addition, `list` is the default opperation, so `wstat set=1,2,3` is enough.

You can also use scientific notation in your set, for example `wstat set=1.1e-2,1.0e-2,0.9` would work fine. You can use a capital or lower-case "E".

## Summarizing a Dataset

Lists are cool and all, but at the end of the day, what you care about is probably more down here. `wstat set=1,2,3 summary` will print something like this:

        These are all rounded values. If you need more precision, use JSON output

        Min               Q1                Med               Q3                Max
        1.000000000000    1.000000000000    2.000000000000    3.000000000000    3.000000000000

        N (Set Size)      Mean              Std. Dev. (s)     Std. Dev. (σ)
        3                 2.000000000000    1.000000000000    0.816496580928

        Possible Outliers:
    
This works just as well in N-Dimensional sets.
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

As of Alpha 7.0.0, linear regression is supported on multidimensional (2+ Dimensioned) sets. Just use `wstat dimensions=3 linreg` for 3 or more dimensions.

Add a `linreg` to your plot command `wstat 2var plot linreg` and it will give you a printout as well as draw the Least Square Regression Line on the graph. `linreg` is a shorthand for `options=linreg`, if you use multiple options you can do `options=option1,linreg,option3` or you can use the shorthands together.

You can also use `linreg` alone, such as `wstat 2var linreg`, which will do the same thing but without making a fancy picture. This is ideal for CLI-only OSes like Ubuntu Server, or when you quite frankly don't care about the pretty pictures. It is also required for 3 or higher dimensions, as you might imagine it is difficult to display a 77-Dimensional plot.

For the set `-2,2,2,0.11134,0,0.8` I got this plot:
![linreg Plot](/demo/linreg.bmp)

And this printout: 
	
	    y=b0 + b1x1 + b2x2 + ... + bnxn

        b0 = 0.9704466666666667
        b1 = -0.472165


        Coefficient of Determination (r^2) = 0.9761489873568633


        Filepath: C:\Users\benny\source\repos\stat\statistics-cli/plots/plot_2019-10-28___08-35-24_PM.bmp
		
Note that this printout is also supported for JSON and CSV output. CSV output does not contain the Coefficient of Determination.
		
## Reexpression

Add a `reexpress`, and then choose either `zscore` or `residual` as an option. `zscore` and `residual` works on N-Dimensional sets. `residual` also requires you specify the regression line to use.

`wstat reexpress zscore` will standardize a 1-Dimensional set.

`wstat reexpress zscore 2var` for 2 dimensions.

`wstat reexpress zscore dimensions=<n>` for n dimensions.

`wstat reexpress residual 2var linreg` will print the residual set for a linear regression line in 2 dimensions.

`wstat reexpress residual dimensions=<n> linreg` will print the residual set for a linear regression line in n dimensions (Since Alpha 7.0.0)

## Normal distribution

Type `wstat cdf` for the cumulative distribution function for a normal distribution. Type `wstat invCdf` for the inverse.

## Quantiles

Type `wstat quantile` to split the set into quantiles. Choose the rank of the quantile with `rank=<n>`. Default rank is 100 (percentiles)

## Linux Users

For drawing plots you will need two packages: `libc6-dev` and `libgdiplus` For some distributions, `xdg-utils` may not come preinstalled, and that is needed for auto-opening images. If you are not using this app for drawing plots, you do not need to install these packages.


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

It should work, but I have no idea how well it will work, I don't have a Mac to test it on, and I am hesitant to pirate OSX and stick it in a VM. Let me know how it goes if you try it.

## 32 Bit, ARM, etc

I have only built x86-64 binaries, but as it is open source, anything that .NET Core will compile on should work. The reason for this, is that I cannot test on a processor that I do not have.

Keep in mind, required packages may differ if you run on 32-bit x86 or on ARM.

## Open Source

This project is a very low priority for me, so if you decide to open some pull-requests to add features or otherwise improve code quality I would be very thankful.

Please note, I would like to have CI sorted out on PRs, however github actions is not currently cooperating with .NET Core 3.0, so we are currently stuck without it.

In addition, everything is subject to code-review

## Licenses

Licenses of all dependencies are included in `/Dependency Licenses/`, any derivative works of this project are to comply with those licenses in addition to the license of this project which is included in `/LICENSE`

---
![Logo](/images/logo_full.png)

# Made by Where 1
