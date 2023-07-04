using System;

namespace RevoltSharp
{

    /// <summary>
    /// Cool badges the user has.
    /// </summary>
    public class UserBadges
    {
        internal UserBadges(ulong value)
        {
            Raw = value;
            Types = (UserBadgeType)Raw;
        }

        /// <summary>
        /// Not recommended to use, use <see cref="Has(UserBadgeType)"/> instead.
        /// </summary>
        public ulong Raw { get; internal set; }

        /// <summary>
        /// Check if a user has a badge.
        /// </summary>
        /// <param name="type">The type of badge to check</param>
        /// <returns><see langword="true" /> if user has this badge otherwise <see langword="false" /></returns>
        public bool Has(UserBadgeType type) => Types.HasFlag(type);

        internal UserBadgeType Types;
    }

    /// <summary>
    /// Cool badges for users :)
    /// </summary>
    [Flags]
    public enum UserBadgeType
    {
        /// <summary>
        /// User is a Revolt developer that works on Revolt magic
        /// </summary>
        Developer = 1,

        /// <summary>
        /// User has helped translate Revolt or other Revolt related stuff.
        /// </summary>
        Translator = 2,

        /// <summary>
        /// User has supported the project by donating.
        /// </summary>
        Supporter = 4,

        /// <summary>
        /// User has disclosed a major bug or security issue.
        /// </summary>
        ResponsibleDisclosure = 8,

        /// <summary>
        /// Hi insert :)
        /// </summary>
        Founder = 16,

        /// <summary>
        /// User has the power to moderate the Revolt instance.
        /// </summary>
        PlatformModeration = 32,

        /// <summary>
        /// Active support for the Revolt project.
        /// </summary>
        ActiveSupporter = 64,

        /// <summary>
        /// OwO
        /// </summary>
        Paw = 128,

        /// <summary>
        /// User was an early member/tester of the Revolt project.
        /// </summary>
        EarlyAdopter = 256,

        /// <summary>
        /// Haha funny
        /// </summary>
        ReservedRelevantJokeBadge1 = 512,

        /// <summary>
        /// Haha memes
        /// </summary>
        ReservedRelevantJokeBadge2 = 1024
    }
}