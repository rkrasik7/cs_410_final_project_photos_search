# cs_410_final_project_photos_search
# author Ron Krasik (rkrasik2)
CS 410 Final Project (My Photos Search)

# Project presentation recording file: "Rec 12-22-17 2"
  https://drive.google.com/file/d/1QJawO76nUeO6yQYVwdw67HH-Wy2BWzZu/view?usp=sharing

Please use Camtasia (recorded using it) or a VLC player in order to watch the recording
  https://www.techsmith.com/video-editor.html
  
  https://www.videolan.org/index.html

# General recommendation
  Please review the documents in Documentation folder

# Link to Installer Download (Windows only!)
  https://www.dropbox.com/s/ef38sas1o7vxji8/CoolPhotoSearchSetup.msi?dl=0

# Source code
  In order to compile and run in Visual Studio 2017 - there is need to uninstall the
  Installer (in case it was installed) and define relevant System Environment variables

# Application high level flow

  1) On App start: load an empty List view and start the background thread as well to fill the list

  2) In a background thread: create Google connection and get a list of object from Google API
  (Metadata only)

  3) Next start a multi-threaded loop and in that loop - bind metadata Item to the UI list

  a. Binding use UI thread (Dispatcher)

  b. As soon as Image Name is binded to the UI element it calls an ImageConverter. In this converter - load
  the actual image using asynchronous process so it won't block the UI thread

  4) After adding the items to the UI - start another multi-threaded loop

  a. This loop also runs in an asynchronous mode

  b. In this loop - call the Google Vision API to get the annotation information

  c. After receiving the info - bind it to the UI text property

  d. Next this is indexed in the Lucene.Net

  5) From here it is pretty straightforward - call the index based on user query input

  # 
  Both the above loop #3 and #4 are wrapped in a while loop which is used for processing 10 items at
  a time to control number of threads running at the same time (+ to improve the UI response)
