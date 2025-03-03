// VRSYS plugin of Virtual Reality and Visualization Group (Bauhaus-University Weimar)
//  _    ______  _______  _______
// | |  / / __ \/ ___/\ \/ / ___/
// | | / / /_/ /\__ \  \  /\__ \ 
// | |/ / _, _/___/ /  / /___/ / 
// |___/_/ |_|/____/  /_//____/  
//
//  __                            __                       __   __   __    ___ .  . ___
// |__)  /\  |  | |__|  /\  |  | /__`    |  | |\ | | \  / |__  |__) /__` |  |   /\   |  
// |__) /~~\ \__/ |  | /~~\ \__/ .__/    \__/ | \| |  \/  |___ |  \ .__/ |  |  /~~\  |  
//
//       ___               __                                                           
// |  | |__  |  |\/|  /\  |__)                                                          
// |/\| |___ |  |  | /~~\ |  \                                                                                                                                                                                     
//
// Copyright (c) 2023 Virtual Reality and Visualization Group
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//-----------------------------------------------------------------
//   Authors:        Tony Jan Zoeppig
//   Date:           2023
//-----------------------------------------------------------------

using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRSYS.Core.Interaction
{
    public class NetworkedXRJoystick : XRJoystick
    {
        #region Member Variables

        private InteractableNetworkState networkState;
        private bool isLocallyGrabbed;

        #endregion

        #region MonoBehaviour Callbacks

        protected override void Awake()
        {
            networkState = GetComponent<InteractableNetworkState>();
            
            base.Awake();
        }

        #endregion

        #region XRInteractable Callbacks

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (networkState != null)
            {
                if (!networkState.isGrabbed.Value)
                {
                    isLocallyGrabbed = true;
                    networkState.UpdateIsGrabbed(true);
                    
                    base.OnSelectEntered(args);
                }
            }
            else
            {
                base.OnSelectEntered(args);
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            if (networkState != null)
            {
                if (isLocallyGrabbed)
                {
                    networkState.UpdateIsGrabbed(false);
                    isLocallyGrabbed = false;
                
                    base.OnSelectExited(args);
                }
            }
            else
            {
                base.OnSelectExited(args);
            }
        }

        #endregion
    }
}

