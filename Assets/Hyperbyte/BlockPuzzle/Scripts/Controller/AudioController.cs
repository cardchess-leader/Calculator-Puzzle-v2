﻿// ©2019 - 2020 HYPERBYTE STUDIOS LLP
// All rights reserved
// Redistribution of this software is strictly not allowed.
// Copy of this software can be obtained from unity asset store only.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperbyte
{
	public class AudioController : Singleton<AudioController>
	{
		[Header("Audio Soureces")]
		public AudioSource audioSource;
		public AudioSource lowSoundSource;

		float lowAudioDefaultVolume = 0.1F;

		[Header("Audio Clips")]
		public AudioClip btnPressSound;
		public AudioClip addGemsSound;
		public AudioClip addGemsSoundChord;

		public void PlayClip(AudioClip clip)
		{
			if (ProfileManager.Instance.IsSoundEnabled)
			{
				audioSource.PlayOneShot(clip);
			}
		}

		public void PlayClipLow(AudioClip clip)
		{
			if (ProfileManager.Instance.IsSoundEnabled)
			{
				lowSoundSource.volume = lowAudioDefaultVolume;
				lowSoundSource.PlayOneShot(clip);
			}
		}

		public void PlayClipLow(AudioClip clip, float volume)
		{
			if (ProfileManager.Instance.IsSoundEnabled)
			{
				lowSoundSource.volume = volume;
				lowSoundSource.PlayOneShot(clip);
			}
		}

		public void PlayButtonClickSound()
		{
			if (ProfileManager.Instance.IsSoundEnabled)
			{
				audioSource.PlayOneShot(btnPressSound);
			}
		}
	}
}

