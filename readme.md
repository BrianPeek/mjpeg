**Project Description**
Library to decode MJPEG streams.  Sample code showing usage is included with the distribution.  For more information, see the full article at Coding4Fun.

* [Coding4Fun Article](http://channel9.msdn.com/coding4fun/articles/MJPEG-Decoder)

MJPEG Decoder v1.2.1
by Brian Peek (http://www.brianpeek.com/)

There are several assemblies in this package.  Select the one that matches the project type you're building:

MjpegProcessor.dll – WinForms and WPF 
MjpegProcessor.winmd - WinRT (AnyCPU, so will work with x86/x64 and ARM)
MjpegProcessorSL.dll – Silverlight (Out of Browser Only!) 
MjpegProcessorWP7.dll – Windows Phone 7 (XNA or Silverlight) 
MjpegProcessorWP8.dll – Windows Phone 8
MjpegProcessorXna4.dll – XNA 4.0 (Windows) 


See the full article at Coding4Fun:
http://channel9.msdn.com/coding4fun/articles/MJPEG-Decoder


Changelog
---------
v1.2.1
	- WinRT fix so response streams are properly disposed
	- The remaining assemblies remain at v1.2

v1.2
	- WinRT support
	- Error event handler
	- Note that the WP7 and XNA assemblies are deprecated, remain at v1.1, and do not contain the Error handler event
v1.1
	- Credentials can be passed to ParseStream (thanks to patricker for the suggestion)
