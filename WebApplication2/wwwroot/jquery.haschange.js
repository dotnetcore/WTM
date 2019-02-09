(function ($, window, undefined) {
	'$:nomunge'; // Used by YUI compressor.

	// Reused string.
	var str_hashchange = 'hashchange',

	  // Method / object references.
	  doc = document,
	  fake_onhashchange,
	  special = $.event.special,

	  // Does the browser support window.onhashchange? Note that IE8 running in
	  // IE7 compatibility mode reports true for 'onhashchange' in window, even
	  // though the event isn't supported, so also test document.documentMode.
	  doc_mode = doc.documentMode,
	  supports_onhashchange = 'on' + str_hashchange in window && (doc_mode === undefined || doc_mode > 7);

	// Get location.hash (or what you'd expect location.hash to be) sans any
	// leading #. Thanks for making this necessary, Firefox!
	function get_fragment(url) {
		url = url || location.href;
		return '#' + url.replace(/^[^#]*#?(.*)$/, '$1');
	};

	$.fn[str_hashchange] = function (fn) {
		return fn ? this.bind(str_hashchange, fn) : this.trigger(str_hashchange);
	};


	$.fn[str_hashchange].delay = 50;
	/*
	$.fn[ str_hashchange ].domain = null;
	$.fn[ str_hashchange ].src = null;
	*/

	special[str_hashchange] = $.extend(special[str_hashchange], {

		// Called only when the first 'hashchange' event is bound to window.
		setup: function () {
			// If window.onhashchange is supported natively, there's nothing to do..
			if (supports_onhashchange) { return false; }

			// Otherwise, we need to create our own. And we don't want to call this
			// until the user binds to the event, just in case they never do, since it
			// will create a polling loop and possibly even a hidden Iframe.
			$(fake_onhashchange.start);
		},

		// Called only when the last 'hashchange' event is unbound from window.
		teardown: function () {
			// If window.onhashchange is supported natively, there's nothing to do..
			if (supports_onhashchange) { return false; }

			// Otherwise, we need to stop ours (if possible).
			$(fake_onhashchange.stop);
		}

	});

	fake_onhashchange = (function () {
		var self = {},
		  timeout_id,

		  // Remember the initial hash so it doesn't get triggered immediately.
		  last_hash = get_fragment(),

		  fn_retval = function (val) { return val; },
		  history_set = fn_retval,
		  history_get = fn_retval;

		// Start the polling loop.
		self.start = function () {
			timeout_id || poll();
		};

		// Stop the polling loop.
		self.stop = function () {
			timeout_id && clearTimeout(timeout_id);
			timeout_id = undefined;
		};

		// This polling loop checks every $.fn.hashchange.delay milliseconds to see
		// if location.hash has changed, and triggers the 'hashchange' event on
		// window when necessary.
		function poll() {
			var hash = get_fragment(),
			  history_hash = history_get(last_hash);

			if (hash !== last_hash) {
				history_set(last_hash = hash, history_hash);

				$(window).trigger(str_hashchange);

			} else if (history_hash !== last_hash) {
				location.href = location.href.replace(/#.*/, '') + history_hash;
			}

			timeout_id = setTimeout(poll, $.fn[str_hashchange].delay);
		};

		return self;
	})();

})(jQuery, this);

