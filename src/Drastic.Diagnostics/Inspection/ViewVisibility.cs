// <copyright file="ViewVisibility.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.Diagnostics.Inspection
{
    /// <summary>
    /// Specifies the visibility of a view.
    /// </summary>
    /// <remarks>
    /// Pretty much all UI toolkits have some notion just like this,
    /// but the WPF names are the best, so stick with them here.
    /// </remarks>
    public enum ViewVisibility : ushort
    {
        /// <summary>
        /// Shown, considered for layout.
        /// </summary>
        Visible = 0,

        /// <summary>
        /// Not shown, considered for layout.
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// Not shown, not considered for layout.
        /// </summary>
        Collapsed = 2,
    }
}
