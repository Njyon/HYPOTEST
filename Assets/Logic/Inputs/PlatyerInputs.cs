//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Logic/Inputs/Inputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputs : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Inputs"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""b0fde70e-7b57-4611-a903-47e8a823dde2"",
            ""actions"": [
                {
                    ""name"": ""VerticalInput"",
                    ""type"": ""Value"",
                    ""id"": ""470a743b-7fa9-4fbc-bc1d-d2760c31ef02"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HorizontalInput"",
                    ""type"": ""Value"",
                    ""id"": ""bfabe92f-0e85-4d05-a440-fdbf91455c88"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""49b41ed4-703e-4260-94c6-eca02cdcbe2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugLevelUp"",
                    ""type"": ""Button"",
                    ""id"": ""6ef7bf84-e454-4171-832b-629ad7cd5324"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DebugLevelDown"",
                    ""type"": ""Button"",
                    ""id"": ""eb641838-751c-49a2-ba4f-57ae34443c02"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AllDebugAreasOn"",
                    ""type"": ""Button"",
                    ""id"": ""5e0df1cb-0f67-44c7-827d-14d41cb73055"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AllDebugAreasOff"",
                    ""type"": ""Button"",
                    ""id"": ""53fd7822-da4e-40ca-b626-b0c6dd77bfbf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponOne"",
                    ""type"": ""Button"",
                    ""id"": ""199f4509-d7b2-48ef-bfdf-62528b0ab23a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponTwo"",
                    ""type"": ""Button"",
                    ""id"": ""66ba1511-b1d3-4df6-b2a8-9e63d0cc59b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponThree"",
                    ""type"": ""Button"",
                    ""id"": ""957c1801-1a81-475f-bccf-5a18660d369b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponFour"",
                    ""type"": ""Button"",
                    ""id"": ""c4585845-71c9-4f77-9e65-6fe5d5f927e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NextWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""273b795b-c9c4-4c8e-8ade-a1ca5e2ecb95"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PreviousWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""7315b547-ee8e-4f01-9636-37d9eb63a8c9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScrollThrouhWeapos"",
                    ""type"": ""Button"",
                    ""id"": ""b696ed97-65f5-438a-8819-97285c6f7056"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""2e62b461-2beb-4455-a316-8afc8efd29bb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WS Axis"",
                    ""id"": ""88a65485-6c88-4a17-9f8f-af107906d7b4"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""3916fe4f-aa48-4871-9ad0-303e88420720"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b7e4c3ae-7edf-4e7c-8c40-50a842861e51"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""UpDown Axis"",
                    ""id"": ""c8be3e4f-e143-49bf-853d-980b1dbe8620"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""64197a01-626f-436f-ae14-2c0b883eb2a2"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f10eee00-0493-4b79-9e22-1196fb678f25"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""VerticalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""AD Axis"",
                    ""id"": ""0adb8c97-dbec-47f7-b2dc-9248fc5de7ff"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ab8c61f0-3fca-42ee-a7ef-c54da5d82ace"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""2b5c2eea-e56e-4057-ae3d-a6b7a9638fa7"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftRight Axis"",
                    ""id"": ""b732e5dd-8c0d-4735-90c5-30b82e0ac9cf"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""289fead3-5add-44e2-9f4d-2aa13e1fb387"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1b8d9c46-eeba-4c68-8a5a-abfcb0d6afbb"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftRightStick"",
                    ""id"": ""33b8bc80-a3df-4f63-91ed-1c5da306d09e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0022f2c2-14f5-4aea-a963-71cefcf650da"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f7b58a7c-5dfa-418e-8d9d-9d74724c5e37"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""HorizontalInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""69b037ab-755a-4379-a020-3bc174f37d8c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce496b26-efb3-4223-a873-98e1da769476"",
                    ""path"": ""<Keyboard>/numpad0"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f22e2172-a57d-479f-884b-28e222fa58c6"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DebugLevelUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9be3b028-3d1e-4c6a-b09e-cc6dcfdf97b5"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DebugLevelDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8bee3f5f-c7d8-4e43-8ca1-2afa8b2d4bcd"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""AllDebugAreasOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5f4bba5-426a-437b-97a0-41a8e4c9c096"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""AllDebugAreasOff"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88b5e357-0fd7-4ac2-bae2-4833ed1ae429"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""WeaponOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92b0cd72-a379-4976-acbd-67cd0a1ef568"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WeaponOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65db29b2-5d64-40e5-b3a6-51be86527ef2"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""WeaponTwo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f948c498-01e0-47ec-a6d5-14da6b73f5bc"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WeaponTwo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d1d8797-27d1-4ed2-86e5-6f2665cfd820"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""WeaponThree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4d7e85b-5977-4f76-a544-59c5e841dcdb"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WeaponThree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""75871d57-3bca-47ae-9e1c-1094cf8049b9"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""WeaponFour"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfc8eabb-f2bc-45c8-9c56-6f997a14609e"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WeaponFour"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68f0a0a4-7921-49ab-ae1f-de6cb75a131c"",
                    ""path"": ""<Mouse>/backButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""NextWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""799a4d71-587d-4439-945b-3bd38ee83f49"",
                    ""path"": ""<Mouse>/forwardButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PreviousWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""MouseScroll"",
                    ""id"": ""95550388-efcc-43b0-9bf2-a92106508280"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScrollThrouhWeapos"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""aab813b9-93a3-4127-87b4-c28cada27d23"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollThrouhWeapos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""75ad7532-a21d-4df7-b117-433b1315be8c"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollThrouhWeapos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""20c51a7b-5d36-42ac-a5b4-2c1b2c435d49"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2a20418-0070-4ed1-b298-cb8e4f2a6b9b"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_VerticalInput = m_Default.FindAction("VerticalInput", throwIfNotFound: true);
        m_Default_HorizontalInput = m_Default.FindAction("HorizontalInput", throwIfNotFound: true);
        m_Default_Jump = m_Default.FindAction("Jump", throwIfNotFound: true);
        m_Default_DebugLevelUp = m_Default.FindAction("DebugLevelUp", throwIfNotFound: true);
        m_Default_DebugLevelDown = m_Default.FindAction("DebugLevelDown", throwIfNotFound: true);
        m_Default_AllDebugAreasOn = m_Default.FindAction("AllDebugAreasOn", throwIfNotFound: true);
        m_Default_AllDebugAreasOff = m_Default.FindAction("AllDebugAreasOff", throwIfNotFound: true);
        m_Default_WeaponOne = m_Default.FindAction("WeaponOne", throwIfNotFound: true);
        m_Default_WeaponTwo = m_Default.FindAction("WeaponTwo", throwIfNotFound: true);
        m_Default_WeaponThree = m_Default.FindAction("WeaponThree", throwIfNotFound: true);
        m_Default_WeaponFour = m_Default.FindAction("WeaponFour", throwIfNotFound: true);
        m_Default_NextWeapon = m_Default.FindAction("NextWeapon", throwIfNotFound: true);
        m_Default_PreviousWeapon = m_Default.FindAction("PreviousWeapon", throwIfNotFound: true);
        m_Default_ScrollThrouhWeapos = m_Default.FindAction("ScrollThrouhWeapos", throwIfNotFound: true);
        m_Default_Attack = m_Default.FindAction("Attack", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_VerticalInput;
    private readonly InputAction m_Default_HorizontalInput;
    private readonly InputAction m_Default_Jump;
    private readonly InputAction m_Default_DebugLevelUp;
    private readonly InputAction m_Default_DebugLevelDown;
    private readonly InputAction m_Default_AllDebugAreasOn;
    private readonly InputAction m_Default_AllDebugAreasOff;
    private readonly InputAction m_Default_WeaponOne;
    private readonly InputAction m_Default_WeaponTwo;
    private readonly InputAction m_Default_WeaponThree;
    private readonly InputAction m_Default_WeaponFour;
    private readonly InputAction m_Default_NextWeapon;
    private readonly InputAction m_Default_PreviousWeapon;
    private readonly InputAction m_Default_ScrollThrouhWeapos;
    private readonly InputAction m_Default_Attack;
    public struct DefaultActions
    {
        private @PlayerInputs m_Wrapper;
        public DefaultActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @VerticalInput => m_Wrapper.m_Default_VerticalInput;
        public InputAction @HorizontalInput => m_Wrapper.m_Default_HorizontalInput;
        public InputAction @Jump => m_Wrapper.m_Default_Jump;
        public InputAction @DebugLevelUp => m_Wrapper.m_Default_DebugLevelUp;
        public InputAction @DebugLevelDown => m_Wrapper.m_Default_DebugLevelDown;
        public InputAction @AllDebugAreasOn => m_Wrapper.m_Default_AllDebugAreasOn;
        public InputAction @AllDebugAreasOff => m_Wrapper.m_Default_AllDebugAreasOff;
        public InputAction @WeaponOne => m_Wrapper.m_Default_WeaponOne;
        public InputAction @WeaponTwo => m_Wrapper.m_Default_WeaponTwo;
        public InputAction @WeaponThree => m_Wrapper.m_Default_WeaponThree;
        public InputAction @WeaponFour => m_Wrapper.m_Default_WeaponFour;
        public InputAction @NextWeapon => m_Wrapper.m_Default_NextWeapon;
        public InputAction @PreviousWeapon => m_Wrapper.m_Default_PreviousWeapon;
        public InputAction @ScrollThrouhWeapos => m_Wrapper.m_Default_ScrollThrouhWeapos;
        public InputAction @Attack => m_Wrapper.m_Default_Attack;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @VerticalInput.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnVerticalInput;
                @VerticalInput.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnVerticalInput;
                @VerticalInput.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnVerticalInput;
                @HorizontalInput.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnHorizontalInput;
                @HorizontalInput.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnHorizontalInput;
                @HorizontalInput.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnHorizontalInput;
                @Jump.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @DebugLevelUp.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelUp;
                @DebugLevelUp.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelUp;
                @DebugLevelUp.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelUp;
                @DebugLevelDown.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelDown;
                @DebugLevelDown.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelDown;
                @DebugLevelDown.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDebugLevelDown;
                @AllDebugAreasOn.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOn;
                @AllDebugAreasOn.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOn;
                @AllDebugAreasOn.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOn;
                @AllDebugAreasOff.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOff;
                @AllDebugAreasOff.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOff;
                @AllDebugAreasOff.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAllDebugAreasOff;
                @WeaponOne.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponOne;
                @WeaponOne.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponOne;
                @WeaponOne.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponOne;
                @WeaponTwo.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponTwo;
                @WeaponTwo.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponTwo;
                @WeaponTwo.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponTwo;
                @WeaponThree.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponThree;
                @WeaponThree.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponThree;
                @WeaponThree.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponThree;
                @WeaponFour.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponFour;
                @WeaponFour.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponFour;
                @WeaponFour.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeaponFour;
                @NextWeapon.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnNextWeapon;
                @PreviousWeapon.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPreviousWeapon;
                @PreviousWeapon.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPreviousWeapon;
                @PreviousWeapon.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPreviousWeapon;
                @ScrollThrouhWeapos.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnScrollThrouhWeapos;
                @ScrollThrouhWeapos.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnScrollThrouhWeapos;
                @ScrollThrouhWeapos.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnScrollThrouhWeapos;
                @Attack.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @VerticalInput.started += instance.OnVerticalInput;
                @VerticalInput.performed += instance.OnVerticalInput;
                @VerticalInput.canceled += instance.OnVerticalInput;
                @HorizontalInput.started += instance.OnHorizontalInput;
                @HorizontalInput.performed += instance.OnHorizontalInput;
                @HorizontalInput.canceled += instance.OnHorizontalInput;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @DebugLevelUp.started += instance.OnDebugLevelUp;
                @DebugLevelUp.performed += instance.OnDebugLevelUp;
                @DebugLevelUp.canceled += instance.OnDebugLevelUp;
                @DebugLevelDown.started += instance.OnDebugLevelDown;
                @DebugLevelDown.performed += instance.OnDebugLevelDown;
                @DebugLevelDown.canceled += instance.OnDebugLevelDown;
                @AllDebugAreasOn.started += instance.OnAllDebugAreasOn;
                @AllDebugAreasOn.performed += instance.OnAllDebugAreasOn;
                @AllDebugAreasOn.canceled += instance.OnAllDebugAreasOn;
                @AllDebugAreasOff.started += instance.OnAllDebugAreasOff;
                @AllDebugAreasOff.performed += instance.OnAllDebugAreasOff;
                @AllDebugAreasOff.canceled += instance.OnAllDebugAreasOff;
                @WeaponOne.started += instance.OnWeaponOne;
                @WeaponOne.performed += instance.OnWeaponOne;
                @WeaponOne.canceled += instance.OnWeaponOne;
                @WeaponTwo.started += instance.OnWeaponTwo;
                @WeaponTwo.performed += instance.OnWeaponTwo;
                @WeaponTwo.canceled += instance.OnWeaponTwo;
                @WeaponThree.started += instance.OnWeaponThree;
                @WeaponThree.performed += instance.OnWeaponThree;
                @WeaponThree.canceled += instance.OnWeaponThree;
                @WeaponFour.started += instance.OnWeaponFour;
                @WeaponFour.performed += instance.OnWeaponFour;
                @WeaponFour.canceled += instance.OnWeaponFour;
                @NextWeapon.started += instance.OnNextWeapon;
                @NextWeapon.performed += instance.OnNextWeapon;
                @NextWeapon.canceled += instance.OnNextWeapon;
                @PreviousWeapon.started += instance.OnPreviousWeapon;
                @PreviousWeapon.performed += instance.OnPreviousWeapon;
                @PreviousWeapon.canceled += instance.OnPreviousWeapon;
                @ScrollThrouhWeapos.started += instance.OnScrollThrouhWeapos;
                @ScrollThrouhWeapos.performed += instance.OnScrollThrouhWeapos;
                @ScrollThrouhWeapos.canceled += instance.OnScrollThrouhWeapos;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IDefaultActions
    {
        void OnVerticalInput(InputAction.CallbackContext context);
        void OnHorizontalInput(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDebugLevelUp(InputAction.CallbackContext context);
        void OnDebugLevelDown(InputAction.CallbackContext context);
        void OnAllDebugAreasOn(InputAction.CallbackContext context);
        void OnAllDebugAreasOff(InputAction.CallbackContext context);
        void OnWeaponOne(InputAction.CallbackContext context);
        void OnWeaponTwo(InputAction.CallbackContext context);
        void OnWeaponThree(InputAction.CallbackContext context);
        void OnWeaponFour(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnPreviousWeapon(InputAction.CallbackContext context);
        void OnScrollThrouhWeapos(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
    }
}
