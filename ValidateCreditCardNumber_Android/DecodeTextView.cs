using System;
using System.Text;
using System.Timers;
using Android.Content;
using Android.Util;
using Android.App;

namespace ValidateCreditCardNumber_Android
{
	public class DecodeTextView : Android.Widget.TextView
	{
		private readonly Timer _timerAnimate = new Timer();
		private TextDecodeEffect _decodeEffect;
		private bool _showing;
		private int _initGenCount;

		public int Interval
		{
			get { return (int)_timerAnimate.Interval; }
			set { _timerAnimate.Interval = value; }
		}

		public DecodeTextView(Context c, IAttributeSet args) : base(c, args)
		{
			_timerAnimate.Interval = 100;
			_timerAnimate.Elapsed += _timerAnimate_Tick;

//			Console.WriteLine ("DecodeTextView executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
		}

		public void AnimateText(bool show, string text, int initGenCount)
		{
			_initGenCount = initGenCount;
			_decodeEffect = new TextDecodeEffect(text) { TextVisible = !show };
			Text = _decodeEffect.Peek (DecodeMode.None);
			_showing = show;
			_timerAnimate.Start ();
		}

		private void _timerAnimate_Tick(object sender, EventArgs e)
		{
			if (_initGenCount != 0) {
				Post (() => {
					Text = _decodeEffect.GenerateNumberRange (Text.Length);
				});
				_initGenCount--;
				return;
			}

			var decodeMode = _showing ? DecodeMode.Show : DecodeMode.Hide;
			var text = _decodeEffect.Peek (decodeMode);

			if (text == null) {
				_timerAnimate.Stop ();
			} else {
				Post (() => {
					Text = text;
				});
			}

//			Console.WriteLine ("_timerAnimate_Tick executing on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
		}
	}

	public enum DecodeMode
	{
		None,
		Show,
		Numbers,
		Hide
	}

	class TextDecodeEffect
	{
		private int _visibleCount;
		private readonly Random _random = new Random ();

		public bool TextVisible
		{
			get { return _visibleCount == OriginalText.Length; }
			set { _visibleCount = value ? OriginalText.Length : 0; }
		}

		public string OriginalText { get; private set; }

		public TextDecodeEffect(string text)
		{
			OriginalText = text;
		}

		public string Peek(DecodeMode mode)
		{
			switch (mode) {
			case DecodeMode.Numbers:
				return GenerateNumberRange (OriginalText.Length);
			case DecodeMode.Hide:
				if (_visibleCount == 0)
					return null;

				_visibleCount--;
				break;
			case DecodeMode.Show:
				if (_visibleCount == OriginalText.Length)
					return null;

				_visibleCount++;
				break;
			}

			var text = GenerateNumberRange (OriginalText.Length - _visibleCount);

			text += OriginalText.Substring (OriginalText.Length - _visibleCount, _visibleCount);

			return text;
		}

		public string GenerateNumberRange(int count)
		{
			var SB = new StringBuilder ();

			for (int i = 0; i < count; i++)
				SB.Append(_random.Next(0, 10));

			return SB.ToString();
		}
	}
}
