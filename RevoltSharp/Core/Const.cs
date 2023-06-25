namespace RevoltSharp;

public static class Const
{
	public const int All_MaxIdLength = 128;
	public const int All_MaxNameLength = 32;
	public const int All_MaxUrlLength = 256;
	public const int Color_MaxLength = 128;

	// Most descriptions and reasons will use this.
	public const int All_MaxDescriptionLength = 1024;

	public const int User_MinNameLength = 2;
	public const int User_MaxStatusTextLength = 128;

	public const int Group_MaxUserIdsListCount = 49;

	public const int Message_MaxSearchListCount = 100;
	public const int Message_MaxDeleteListCount = 100;
	public const int Message_MaxEmbedsListCount = 10;
	public const int Message_MaxContentLength = 2000;
	public const int Message_EmbedTitleMaxLength = 100;

	public const int Message_ReactionsMaxCount = 20;
}
