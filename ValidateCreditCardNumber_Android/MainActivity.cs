﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text;
using System.Timers;
using System.Linq;

namespace ValidateCreditCardNumber_Android
{
	[Activity (Label = "ValidateCreditCardNumber_Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		EditText editText;
		DecodeTextView resultLabel;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			editText = FindViewById<EditText> (Resource.Id.editText);
			Button validateButton = FindViewById<Button> (Resource.Id.validateButton);
			resultLabel = FindViewById<DecodeTextView> (Resource.Id.resultLabel);

			editText.KeyListener = Android.Text.Method.DigitsKeyListener.GetInstance("0123456789" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
			
			validateButton.Click += OnNumberEntryCompleted;
		}

		void OnNumberEntryCompleted(object sender, EventArgs args)
		{
			var entry = editText;
			var resultText = "";

			if (Mod10Check (entry.Text)) {
//				resultLabel.SetTextColor(Android.Graphics.Color.White);
				resultText = "__VALID NUMBER";
			} else {
				resultText = "INVALID NUMBER";
			}

//			entry.Enabled = false;
//			resultLabel.AnimateText (true, resultText, 10);
			RunOnUiThread(() => resultLabel.AnimateText (true, resultText, 10));
		}

		public static bool Mod10Check(string creditCardNumber)
		{
			// Check whether input string is null or empty.
			if (string.IsNullOrEmpty(creditCardNumber)) {
				return false;
			}

			char[] charArray = creditCardNumber.ToCharArray();

			// 1. Starting with the check digit double the value of every other digit 
			// 2. If doubling of a number results in a two digits number, add up.
			//    the digits to get a single digit number. This will results in eight single digit numbers.
			// 3. Get the sum of the digits.
			int sumOfDigits = charArray.Where((e) => e >= '0' && e <= '9')
				.Reverse()
				.Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
				.Sum((e) => e / 10 + e % 10);


			// If the final sum is divisible by 10, then the credit card number.
			// is valid. If it is not divisible by 10, the number is invalid.            
			return sumOfDigits % 10 == 0;
		}
	}
}
