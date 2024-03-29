﻿# Image Bulk Renamer

## Synopsis

This is a simple application to allow an arbitrary-size collection of image (jpg) files to be renamed using a unified file naming system.

I use this application as part of my simple photograph archiving and processing workflow. I couldn't find something that did exactly
what I wanted, so I built my own solution. The current version does just what I need and no more. 


## Installation and Use

The application is a C#/WinForms program that compiles to a exe that requires .net 6.
Download a (release)[https://github.com/andyjohnson0/ImageBulkRenamer/releases], or clone the repo and build it yourself.

Click File->Open and select a directory. A preview list will then be generated showing how the files will be renamed. Click the Rename
button to rename all the files, or double-click an invividual list item to override the naming.

Files are named using the embedded EXIF timestamp, and have the format YYYYMMDD_HHMMSS.jpg. Each file's creation and last-modified
timestamps are also set to the EXIF timestamp. The timr portion of the timestamp is interpreted as a local time.

If your image files have accompanying xmp "sidecar" metadata files then tick the "Rename XMP sidecar files" checkbox to ensure that
the sidecars are renamed along with the images. Sidecars must have a filename of the form <imagefilename>.<imageext>.xmp.

The program is carefully written to make data-loss impossible. It uses File.Move() *without* the overwrite flag, which should make it
impossible to inadvertantly delete any file. If a target filename already exists then any files to be given the same name are simply
skipped. Nevertheless, it is advisable to back-up important files before using the application. Never work on the only copy of 
important files.


## Author

Andrew Johnson | andy@andyjohnson.uk | http://andyjohnson.uk


## Licence

The MIT License (MIT)

Copyright (c) 2014 Andrew Johnson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
