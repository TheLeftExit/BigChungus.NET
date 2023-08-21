﻿namespace BigChungus.Unmanaged;

[Flags]
public enum NMTBHOTITEM_FLAGS : uint
{
    HICF_OTHER = 0U,
    HICF_MOUSE = 1U,
    HICF_ARROWKEYS = 2U,
    HICF_ACCELERATOR = 4U,
    HICF_DUPACCEL = 8U,
    HICF_ENTERING = 16U,
    HICF_LEAVING = 32U,
    HICF_RESELECT = 64U,
    HICF_LMOUSE = 128U,
    HICF_TOGGLEDROPDOWN = 256U,
}