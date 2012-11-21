// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
	"use strict";

	WinJS.Binding.optimizeBindingReferences = true;

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;
	var mjpeg = new MjpegProcessor.MjpegDecoder();

	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
				// TODO: This application has been newly launched. Initialize
				// your application here.
			} else {
				// TODO: This application has been reactivated from suspension.
				// Restore application state here.
			}
			document.getElementById("start").addEventListener("click", startHandler);
			args.setPromise(WinJS.UI.processAll());
		}
	};

	var startHandler = function (ev) {
		mjpeg.onframeready = frameReadyHandler;
		mjpeg.onerror = errorHandler;
		mjpeg.parseStream(new Windows.Foundation.Uri("http://192.168.2.200/img/video.mjpeg"));
	};

	var frameReadyHandler = function (ev) {
		var reader = Windows.Storage.Streams.DataReader.fromBuffer(ev.frameBuffer);
		var bytes = new Uint8Array(ev.frameBuffer.length);
		reader.readBytes(bytes);
		var blob = new Blob([bytes], { type: 'image/jpeg' });
		document.getElementById('img').src = URL.createObjectURL(blob, { oneTimeOnly: true });
	};

	var errorHandler = function (error) {
		new Windows.UI.Popups.MessageDialog(error.message).showAsync();
	};

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state
		// that needs to persist across suspensions here. You might use the
		// WinJS.Application.sessionState object, which is automatically
		// saved and restored across suspension. If you need to complete an
		// asynchronous operation before your application is suspended, call
		// args.setPromise().
	};

	app.start();
})();
