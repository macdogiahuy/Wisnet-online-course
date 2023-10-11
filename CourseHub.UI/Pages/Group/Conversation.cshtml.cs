using CourseHub.Core.Models.User.UserModels;
using CourseHub.Core.RequestDtos.Social.ConversationDtos;
using CourseHub.UI.Helpers.Http;
using CourseHub.UI.Helpers;
using CourseHub.UI.Services.Contracts.SocialServices;
using CourseHub.UI.Services.Contracts.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CourseHub.Core.Models.Social;
using CourseHub.Core.RequestDtos.Social.ChatMessageDtos;
using CourseHub.UI.Services.Implementations.UserServices;

namespace CourseHub.UI.Pages.Group;

public class ConversationModel : PageModel
{
    public UserFullModel Client { get; set; }
    public Dictionary<Guid, UserOverviewModel> RelatedUsers { get; set; }
    public List<Core.Models.Social.ConversationModel> Conversations { get; set; }
    public Core.Models.Social.ConversationModel CurrentConversation { get; set; }
    public List<ChatMessageModel> Messages { get; set; }



    public async Task<IActionResult> OnGet(
        [FromQuery] Guid id,
        [FromServices] IUserApiService userApiService,
        [FromServices] IConversationApiService conversationApiService,
        [FromServices] IChatMessageApiService chatMessageApiService)
    {
        if (id == default)
            return Redirect(Global.PAGE_404);

        Client = await HttpContext.GetClientData();
        if (Client is null)
            return Redirect(Global.PAGE_SIGNIN);



        QueryConversationDto dto = new()
        {
            ByClient = true
        };
        var conversations = await conversationApiService.GetAsync(dto, HttpContext);
        Conversations = conversations.Items;

        List<Guid> relatedUserIds = new();
        foreach (var conversation in Conversations)
            foreach (var member in conversation.Members)
                relatedUserIds.Add(member.CreatorId);
        var relatedUsers = await userApiService.GetOverviewAsync(relatedUserIds);
        RelatedUsers = relatedUsers.ToDictionary(_ => _.Id);

        foreach (var conversation in Conversations)
        {
            if (conversation.Title == string.Empty)
            {
                var otherMember = conversation.Members.FirstOrDefault(_ => _.CreatorId != Client.Id);
                RelatedUsers.TryGetValue(otherMember.CreatorId, out var user);

                if (user is null)
                    continue;
                conversation.Title = user.FullName;

                if (string.IsNullOrEmpty(conversation.AvatarUrl))
                    conversation.AvatarUrl = user.AvatarUrl;
            }
        }

        CurrentConversation = conversations.Items.FirstOrDefault(_ => _.Id == id)!;
        QueryChatMessageDto queryChatMessageDto = new()
        {
            ConversationId = id
        };
        var messages = await chatMessageApiService.GetAsync(queryChatMessageDto, HttpContext);
        Messages = messages.Items;

        Client.AvatarUrl = UserApiService.GetAvatarApiUrl(Client.AvatarUrl, Client.Id);
        return Page();
    }
}
