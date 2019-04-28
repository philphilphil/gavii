# gavii
gavii generates a static website for collections of things based on images.
![](gavii_example.gif)
## Creating a new website
Create a new site with "gavii new SiteName". gavii will create a new folder named SiteName with the following strucutre inside:

For layout of the website:
- Layout //Where the html layout lays
- - _Layout.html //Base Layout
- - _Gallery.html //Used to generate the gallery on the index page
- - _Page.html //Layout for pages
- - _Post.html //Layout for posts
- - style.css //css file

For website content
- pages //For each page one html file
- posts //for each post one folder
- - post.html //settings and text of the post go here

## Adding a page
Copy a existing page and change its content and name. Currently pages are not automatically added to the navigation. Just add a link to the page on a desired location in Layout/_Layout.html. Links to pages will be /$Name.

## Adding a Post
Copy paste the "-EmptyPost"-Folder, rename it to a new name. Edit the post.html.
### Adding Images
To add images to a post just drop them into the post folder. The gallery image will be the first one sorted by name.


### General things
- Post folders with a starting "-" will be ignored
- Posts are sorted by date, newest first
- GPS and Tags are not yet working

## Generating the website
Just call gavii in a website directory (the one which was created with the "new" command). Output will be generated and put into "Output"

# Installation
## Windows
Download the release for windows (or build it yourself in a self contained release build).
Put the files in a sensible folder (eg. C:\Program Files\gavii). [Add the folder to your path.](https://docs.microsoft.com/en-us/previous-versions/office/developer/sharepoint-2010/ee537574(v%3Doffice.14))
## Linux
Unpack, place somewhere sensible and add to your ~/.bashrc with "export PATH=$PATH:</path/to/gavii>"
