using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for categories.
/// </summary>
public static class CategoryHelper
{
    public static Task AddCategoryAsync(this Server server, string name, int position = 0)
        => AddServerCategoryAsync(server.Client.Rest, server, name, position);

    public static async Task AddServerCategoryAsync(this RevoltRestClient rest, Server server, string name, int position = 0)
    {
        Conditions.ServerIdLength(server.Id, nameof(AddServerCategoryAsync));

        await server.CategoryLock.WaitAsync();
        ModifyServerRequest Req = new ModifyServerRequest();
        List<CategoryJson> Categories = server.Categories.Select(x => x.ToJson()).ToList();
        Categories.Insert(position, new CategoryJson
        {
            id = Ulid.NewUlid().ToString(),
            title = name,
            channels = Array.Empty<string>()
        });
        Req.categories = Optional.Some(Categories);
        try
        {
            await rest.PatchAsync<ServerJson>($"/servers/{server.Id}", Req);
        }
        finally
        {
            server.CategoryLock.Release();
        }
    }

    public static Task RemoveAsync(this ServerCategory category)
        => RemoveServerCategory(category.Client.Rest, category.Server, category.Id);

    public static async Task RemoveServerCategory(this RevoltRestClient rest, Server server, string categoryId)
    {
        Conditions.ServerIdLength(server.Id, nameof(AddServerCategoryAsync));

        await server.CategoryLock.WaitAsync();

        ModifyServerRequest Req = new ModifyServerRequest();
        List<CategoryJson> Categories = server.Categories.Where(x => x.Id != categoryId).OrderBy(x => x.Position).Select(x => x.ToJson()).ToList();
        Req.categories = Optional.Some(Categories);
        

        try
        {
            await rest.PatchAsync<ServerJson>($"/servers/{server.Id}", Req);
        }
        finally
        {
            server.CategoryLock.Release();
        }
    }

    public static Task ModifyAsync(this ServerCategory category, Option<string> name = null, Option<string[]> channels = null, Option<int> position = null)
        => ModifyServerCategoryAsync(category.Client.Rest, category.Server, category.Id, name, channels, position);

    public static async Task ModifyServerCategoryAsync(this RevoltRestClient rest, Server server, string categoryId, Option<string> name = null, Option<string[]> channels = null, Option<int> position = null)
    {
        Conditions.ServerIdLength(server.Id, nameof(ModifyServerCategoryAsync));

        await server.CategoryLock.WaitAsync();

        ModifyServerRequest Req = new ModifyServerRequest();
        List<CategoryJson> Categories = new List<CategoryJson>();

        CategoryJson? SelectedCategory = null;
        foreach (var i in server.Categories.OrderBy(x => x.Position))
        {
            if (categoryId == i.Id)
            {
                CategoryJson Cat = new CategoryJson
                {
                    id = i.Id,
                    title = i.Name,
                    channels = i.ChannelIds
                };

                if (name != null)
                    Cat.title = name.Value;
                if (channels != null)
                    Cat.channels = channels.Value;

                Categories.Add(Cat);

                SelectedCategory = Cat;
            }
            else
                Categories.Add(i.ToJson());
        }
        if (position != null)
        {
            Console.WriteLine("Update pos");
            if (SelectedCategory != null)
            {
                Console.WriteLine("Insert");
                Categories.Remove(SelectedCategory);
                Categories.Insert(position.Value, SelectedCategory);
            }
        }
        foreach(var i in Categories)
        {
            Console.WriteLine("Update: " + i.title);
        }

        Req.categories = Optional.Some(Categories);


        try
        {
            await rest.PatchAsync<ServerJson>($"/servers/{server.Id}", Req);
        }
        finally
        {
            server.CategoryLock.Release();
        }
    }
}
