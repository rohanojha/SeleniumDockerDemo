# SeleniumDockerDemo
 
_Automation testing with a C# executable and deploying it to Docker_

An honest confession at the very beginning, I love working in the .Net environment and especially in the Console/Windows Forms/WPF &quot;sector&quot;. I know those are so pre-2010 technologies but I guess I started my career with them and have been a fan ever since.

So when I moved to Automation testing sphere, I have always tried and gone extra step to use those. Some of them being:

1. Using Selenium with C#, rather than Java (this doesn&#39;t look like a big thing but the AUT was under Java )
2. I have avoided NUnit almost all of my automation career, just so that I can create an &quot;exe&quot; which &quot;runs&quot; on machines
3. For one of my projects which was/is heavily API services oriented, rather than using a tool like SoapUI which would have made life easier, I decided to create my own &quot;Framework&quot; and then go back and re-invent the wheel.
4. Dissed Java, just because it can run on Linux and MAC in addition to Windows
5. Avoided learning newer technologies like DOCKER cos I thought they were not compatible with C# apps.

But #5 was until the COVID-19 pandemic. With the time available in hand, I decided to see if my beloved &quot;C# exe&quot; can also be ported to Docker. It also helped that I hated the fact that I would have to work on porting my code to Java or Python for it to be Docker-ised

**Spoiler warning:** IT does and guess what it does it gloriously.

_Problem Statement_

For this document perspective, let&#39;s chalk out our problem statement/ mission

1. Have a simple automation suite written in C#
2. Suite should have an external data file (Excel file in this example)
3. Suite should spit out execution results at the end
4. Have the above suite in a Docker image
5. Run a container with the image created at #4
6. Retrieve results created in the run and store it in our local machine

So what I plan to do in this example, is have a simple automation script based on Selenium C# and get it docker-ised.

_What we need?_

Since it&#39;s a fairly straight-forward thing, we require only the following:

1. Docker for Windows
2. Visual Studio 2017 or later
3. .Net Core SDK
4. Selenium libraries

_Getting Started_

First things first, in my limited knowledge you can&#39;t create Docker images from projects build on .Net Framework 4/4.5.

So make sure whenever you create your project have it created as a .Net Core application

![](RackMultipart20200528-4-1f0remb_html_612d0864a2daaea8.png)

Import your libraries and have your code ready. Make sure it runs on your machines before we even begin to port it to Docker.

So my code to run it in my local machine looks something like this

![](RackMultipart20200528-4-1f0remb_html_5a33957d44325b03.png)

There is an excel sheet which has following data

![](RackMultipart20200528-4-1f0remb_html_eefc21bc845d67f4.png)

When the script runs it verifies the title from excel to the one in the browser title and marks TC as Pass and Fail… it takes screenshot as well and stores in a timestamped folder

![](RackMultipart20200528-4-1f0remb_html_53388c3fc94d9b3f.png)

Contents of the folder :

![](RackMultipart20200528-4-1f0remb_html_309d95f826e62801.png)

My report file looks like this:

![](RackMultipart20200528-4-1f0remb_html_a92e3dcfc18cab78.png)

_Step 1 - Dockerfile_

Now that we have code which runs on local machine, let&#39;s start the process to make it Docker compatible.

First step is to create a Docker file.

Qs: Well what&#39;s a docker file?

Ans: As per the definition from Docker&#39;s official website [here](https://docs.docker.com/engine/reference/builder/) ,

&quot;_A Dockerfile is a text document that contains all the commands a user could call on the command line to assemble an image. Using docker build users can create an automated build that executes several command-line instructions in succession. This page describes the commands you can use in a Dockerfile&quot;_

In a nutshell, Dockerfile is the utility which lets you execute commands you want to run before you actually try to &quot;run/execute the application &quot;. This might include (but not limited to):

1. Downloading the pre-requisites (.Net Core SDK in the docker environment)
2. Copying over your source code to Docker image
3. Copying any additional files (e.g. : certs ,licenses) which might not be part of the build but are required
4. Downloading a file over the internet (e.g. : in our case Test Data sheet)
5. Compiling the project
6. Running the actual executable

Though it might be a wrong analogy, I like to consider Dockerfile to be very similar to a &quot;.bat&quot; file in the Windows environment.

Always remember whenever you do &quot;BUILD&quot; in a docker eco-system, it&#39;s always the Dockerfile which is executed

So first let&#39;s list down the things we need in our image to run

1. .Net Core SDK
2. Chrome
3. Chrome drivers
4. Our code
5. Data file

So our Dockerfile should have commands to get all of the above pre-requisites done

1. First line is to get the Docker image to know that the code is built on which SDK and download it

![](RackMultipart20200528-4-1f0remb_html_157cc5bd54334b31.png)

Since I am using VS 2017, my code was built on

![](RackMultipart20200528-4-1f0remb_html_4e12e0acfc6bd801.png)

1. Getting chrome installed

![](RackMultipart20200528-4-1f0remb_html_7b4ed11218d995b9.png)

The above commands downloads the Google talk plugin and installs Chrome on the image

1. Now let&#39;s add commands to download ChromeDriver to run our execution

![](RackMultipart20200528-4-1f0remb_html_3acf35fba4645c9e.png)

Note: For Steps 2 and 3 above, I am really thankful to [Pablo Assis](https://github.com/devpabloassis) for his excellent docker file [here](https://github.com/devpabloassis/seleniumdotnetcore/blob/master/Dockerfile).

Also another point to note is that Step 2 and 3 remain the same, no matter what is the Programming Language you would be using to script your automation test cases.

1. Now that we have the Chrome and ChromeDriver, let&#39;s get our code from local machine to the image.

![](RackMultipart20200528-4-1f0remb_html_aedec4a4d6292ee0.png)

This copies over all the files from the folder your Dockerfile is located in to the &quot;src&quot; folder in the image.

1. Now let&#39;s download the test data file from the internet over to our image

![](RackMultipart20200528-4-1f0remb_html_8036e131ee09ef8f.png)

1. Now let&#39;s compile our code and have an executable created with the Docker command line

![](RackMultipart20200528-4-1f0remb_html_ef98df75e1bb980c.png)

_DOCKER FILE_

FROM mcr.microsoft.com/dotnet/core/sdk:2.1

ADD https://dl.google.com/linux/direct/google-talkplugin\_current\_amd64.deb /src/google-talkplugin\_current\_amd64.deb

# Install Chrome

RUN apt-get update &amp;&amp; apt-get install -y \

apt-transport-https \

ca-certificates \

curl \

gnupg \

hicolor-icon-theme \

libcanberra-gtk\* \

libgl1-mesa-dri \

libgl1-mesa-glx \

libpango1.0-0 \

libpulse0 \

libv4l-0 \

fonts-symbola \

--no-install-recommends \

&amp;&amp; curl -sSL https://dl.google.com/linux/linux\_signing\_key.pub | apt-key add - \

&amp;&amp; echo &quot;deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main&quot; \&gt; /etc/apt/sources.list.d/google.list \

&amp;&amp; apt-get update &amp;&amp; apt-get install -y \

google-chrome-stable \

--no-install-recommends \

&amp;&amp; apt-get purge --auto-remove -y curl \

&amp;&amp; rm -rf /var/lib/apt/lists/\*

# Download ChromeDriver

RUN set -x \

&amp;&amp; apt-get update \

&amp;&amp; apt-get install -y --no-install-recommends \

ca-certificates \

curl \

unzip \

&amp;&amp; rm -rf /var/lib/apt/lists/\* \

&amp;&amp; curl -sSL &quot;https://dl.google.com/linux/direct/google-talkplugin\_current\_amd64.deb&quot; -o /tmp/google-talkplugin-amd64.deb \

&amp;&amp; dpkg -i /tmp/google-talkplugin-amd64.deb \

&amp;&amp; mkdir \opt\selenium \

&amp;&amp; curl -sSL &quot;https://chromedriver.storage.googleapis.com/2.40/chromedriver\_linux64.zip&quot; -o /tmp/chromedriver.zip \

&amp;&amp; unzip -o /tmp/chromedriver -d /opt/selenium/ \

&amp;&amp; rm -rf /tmp/\*.deb \

&amp;&amp; apt-get purge -y --auto-remove curl unzip

# Add chrome user

RUN groupadd -r chrome &amp;&amp; useradd -r -g chrome -G audio,video chrome \

&amp;&amp; mkdir -p /home/chrome/Downloads &amp;&amp; chown -R chrome:chrome /home/chrome

WORKDIR /src

COPY . .

ADD &quot;https://srv-file19.gofile.io/download/pWOMw4/sample.xlsx&quot; /src

RUN dotnet build -c Release

ENTRYPOINT [&quot;dotnet&quot;, &quot;run&quot;]

_Changes to the code_

Now since our automation would run on a Docker container, there would be changes required in our code to accommodate that.

Some of the changes which I would like to point out especially are

1. Chrome would have to be run in &quot;headless&quot; mode
2. We would be using the Docker downloaded binaries related to ChromeDriver

So our code changes from

![](RackMultipart20200528-4-1f0remb_html_c3c774e6e73b3497.png)

to something like this

![](RackMultipart20200528-4-1f0remb_html_ee49223112a3c837.png)

1. Make sure the any path in your code are relative. Also since in the Dockerfile command #4 we had set Working Directory to src, all of the folders created henceforth will be inside the &quot;src&quot; folder

![](RackMultipart20200528-4-1f0remb_html_25d308487bbf6d31.png)

E.g.: The above code creates a folder &quot;Results&quot; in the &quot;src&quot; folder , where we can store our results and screenshots.

_Docker Image creation_

Now that we are ready, let&#39;s build the docker image first

1. Open up PowerShell/CMD in admin mode
2. Navigate to the location of the docker file (using cd)
3. Use this command to build your docker image with the name &quot;seldockerdemo&quot;

_ **PS C:\Users\rojha\source\repos\SeleniumDemoDocker\SeleniumDemoDocker\&gt; docker build --tag seldockerdemo .** _

1. Now all the files required would be downloaded, code files copied, code compiled and your image would be ready.

![](RackMultipart20200528-4-1f0remb_html_41cda136de007389.png)

_Container and Execution_

Now that we have the image ready, all we want to do is run the image in a container and get back the execution results from the Docker container

1. Let&#39;s create a container with the image created above and since the image is built with RUN (as per Dockerfile), it will execute our automation code.

_ **PS C:\Users\rojha\source\repos\SeleniumDemoDocker\SeleniumDemoDocker\&gt; docker run -e TZ=America/Chicago --name RohanTest -it seldockerdemo** _

1. Above command creates a container with name RohanTest and executes the image &quot;seldockerdemo&quot;

1. Also it sets the time zone to CST (my local time zone). It&#39;s not important and can be skipped, however I like my results/logs to be in the same time zone as mine (personal preference)

![](RackMultipart20200528-4-1f0remb_html_1f2d032909f0d0d9.png)

1. You will see execution logs on the command line

1. Now let&#39;s see how the execution went and copy over the results to our local machine.

_ **PS C:\Users\rojha\source\repos\SeleniumDemoDocker\SeleniumDemoDocker\&gt; docker cp RohanTest:/src C:\src** _

The above command would copy over the &quot;src&quot; folder from the container over to my local machine&#39;s &quot;C:\src&quot; folder (I copy the whole folder, to cross-check the code file version)

![](RackMultipart20200528-4-1f0remb_html_3b75bb5d2ffae3b5.png)

![](RackMultipart20200528-4-1f0remb_html_30bcf6690ff3b9ea.png)

Notice the HostName in the result file, it&#39;s the same as Container name

![](RackMultipart20200528-4-1f0remb_html_82936db824516e98.png)

So hurray, we have run out .Net automation code for Selenium on Docker.

I know it was a very simple example, but the only intention of this was to show that it can be DONE.

**Noob Tip:** Create a batch file (.bat) and have all the commands one after the other, to run the image in container…copy the results... and then finally delete the container.

I will put this whole project in GitHub for anybody who might be interested.
