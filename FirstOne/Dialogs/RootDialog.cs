using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Threading.Tasks;

namespace FirstOne.Dialogs
{
    [LuisModel("ffd33b5c-0c12-42a3-9190-90e7f7ffa9f3", "50c4e409aa254de3a6f3749dbc8e3f18")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        int qno, ansind, ansindf;
        List<string> keys;

        private String[] ans = new String[13];
        private String[] finalans = new String[110];
        private String[] finalans1 = new String[120];
        private String[] images = { "fur.jpg", "feather.png", "layEggs.jpg", "feed.png", "fly.jpg", "aquatic.jpg", "predator.jpg", "tooth.jpg", "poisonous.jpg", "swim.jpg", "tail.gif", "domestic.jpg" };
        private String[] s = { "characters with fur be like", "characters with feathers be like", "characters that lay eggs be like", "feeding babies with milk be like", "characters flying be like", "characters in water be like", "predators be like", "characters with tooth be like", "poisonous characters be like", "characters that swim be like", "characters having tail be like", "pets be like" };
        private IEnumerable<string> matches_list = new List<string>();
        int k = 0, f = 0;
        private object activity;
        int imgidx = 0;
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Greetings")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var cardMsg = context.MakeMessage();
            var attachment = BotWelcome("", "");
            cardMsg.Attachments.Add(attachment);
            await context.PostAsync(cardMsg);
            await context.PostAsync("**To The GUESS WHO BOT**");

            await Task.Delay(1000);
            await context.PostAsync("Wanna know about me.......type **About**");
            await context.PostAsync("To start game type **start**");
        }

        [LuisIntent("about")]
        public async Task About(IDialogContext context, LuisResult result)
        {


            await context.PostAsync("GuessWhoBot can read your mind and tell you what character you are thinking about, just by asking a few questions related to that character.Playing with this bot need some knowledge on the specified categories");
            await context.PostAsync("*To start the game type **Start**");
        }

        [LuisIntent("quit")]
        public async Task quit(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(context, QuitAsync, "Do you really want to quit ?");
            //context.Done(quit);


        }

        private async Task QuitAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            if (confirmation)
            {
                await context.PostAsync("**see u soon :)** ");
                await context.PostAsync("**Have a nice day**");

            }
            else
            {
                await context.PostAsync("**Let's Play........**");
                var options = new Selection[] { Selection.Animals_and_Birds, Selection.Fruits_and_Vegetables };
                var descriptions = new string[] { "Animals and Birds", "Fruits and Vegetables" };
                PromptDialog.Choice<Selection>(context, categorySelection, options, "Which selection do you want?", descriptions: descriptions);
            }

        }

        [LuisIntent("StartGame")]
        public async Task startGame(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("**Let's Play ............**");
            var options = new Selection[] { Selection.Animals_and_Birds, Selection.Fruits_and_Vegetables };
            var descriptions = new string[] { "Animals and Birds", "Fruits and Vegetables" };
            PromptDialog.Choice<Selection>(context, categorySelection, options, "Which selection do you want?", descriptions: descriptions);

            //PromptDialog.Confirm(context, startgame, "Do you want to play with me ?");

        }
        private async Task startgame(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            if (confirmation)
            {
                await context.PostAsync("**Let's Play ............**");
                var options = new Selection[] { Selection.Animals_and_Birds, Selection.Fruits_and_Vegetables };
                var descriptions = new string[] { "Animals and Birds", "Fruits and Vegetables" };
                PromptDialog.Choice<Selection>(context, categorySelection, options, "Which selection do you want?", descriptions: descriptions);
            }
            else
                await context.PostAsync("**OK :) Have A Nice Day**");
        }

        private enum Selection
        {
            Animals_and_Birds, Fruits_and_Vegetables
        }

        private enum Guessed
        {
            right, wrong
        }

        private async Task confirmation(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;

        }

        private async Task categorySelection(IDialogContext context, IAwaitable<Selection> result)
        {
            var selection = await result;


            if (selection.ToString().Equals("Animals_and_Birds"))
            {
                await context.PostAsync($"**OK :)   Think About an Animal or a Bird**");
                Methods rm = new Methods();
                JToken r = rm.getData();
                JObject inner = r[1].Value<JObject>();
                keys = inner.Properties().Select(p => p.Name).ToList();
                await AfterResetAsync(context, keys);
            }
            else
            {
                await context.PostAsync($"**OK :)   Think About a Fruit or a Vegetable**");
                Methods m = new Methods();
                JToken r1 = m.getveg();
                JObject inner = r1[1].Value<JObject>();
                keys = inner.Properties().Select(p => p.Name).ToList();
                await Receivedquestions(context, keys);


            }
        }


        private async Task Receivedquestions(IDialogContext context, List<string> keys)
        {
            qno = 0;
            PromptDialog.Confirm(context, OnquestionReplyforVeg, keys[qno]);

        }

        private async Task OnquestionReplyforVeg(IDialogContext context, IAwaitable<bool> result)
        {
            var answer = await result;
            ansindf = 0;
            if (answer)
                ans[qno] = "Yes";
            else
                ans[qno] = "No";
            int c = 0, i = 0;
            string propertyValue = "";
            qno++;

            if (qno == 11)
            {
                ansindf = 0;
                Methods rm = new Methods();
                JArray r = rm.getValuesofveg();
                foreach (JObject parsedObject in r.Children<JObject>())
                {
                    c = 0;
                    i = 0;
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        propertyValue = (string)parsedProperty.Value;
                        if (propertyValue.Equals(ans[i]))
                        {
                            c++;
                            i++;
                        }
                    }
                    if (c == 11)
                    {
                        try
                        {
                            finalans1[f++] = propertyValue;
                            var options = new Guessed[] { Guessed.right, Guessed.wrong };
                            var descriptions = new string[] { "Am I Right", "Am I Wrong" };
                            PromptDialog.Choice<Guessed>(context, CheckAnswerAsync2, options, "Is Your Item......." + finalans1[ansindf] + "", descriptions: descriptions);


                        }
                        catch (Exception e)
                        {
                            // await context.PostAsync("error------->"+e.ToString());
                        }
                    }
                }
                if (f == 0)
                {
                    await context.PostAsync("**No Such Vegetable or Fruit Exists**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");
                }


            }
            else if (qno < 11)
            {

                PromptDialog.Confirm(context, OnquestionReplyforVeg, keys[qno]);

            }
            else
                context.Wait(MessageReceived);

        }

        private async Task CheckAnswerAsync2(IDialogContext context, IAwaitable<Guessed> result)
        {
            var selection = await result;

            ansindf++;
            if (ansindf < f+1)
            {
                if (selection.ToString().Equals("right"))
                {
                    await context.PostAsync("**Great, guessed right one more time !**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");
                }
                else if(ansindf < f)
                {
                    try
                    {
                        var options = new Guessed[] { Guessed.right, Guessed.wrong };
                        var descriptions = new string[] { "Am I Right", "Am I Wrong" };
                        PromptDialog.Choice<Guessed>(context, CheckAnswerAsync2, options, "Is Your item......." + finalans1[ansindf], descriptions: descriptions);
                    }
                    catch (Exception e)
                    {

                    }

                }
                else
                {
                    await context.PostAsync("**No Such Vegetable or Fruit Exists**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");

                }
            }
            else
            {
                var cardMsg = context.MakeMessage();
                var attachment = bravo("", "");
                cardMsg.Attachments.Add(attachment);
                await context.PostAsync(cardMsg);


                await context.PostAsync("**Bravo!!!You Defeated Me**");
                PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");

            }

        }

        public async Task AfterResetAsync(IDialogContext context, List<String> keys)
        {
            qno = 0;
            var cardMsg = context.MakeMessage();
            var attachment = GetThumbnailCard(s[imgidx], "");
            cardMsg.Attachments.Add(attachment);
            await context.PostAsync(cardMsg);

            PromptDialog.Confirm(context, OnquestionReply, keys[qno], attachment.ToString());

        }

        private async Task OnquestionReply(IDialogContext context, IAwaitable<bool> result)
        {
            var answer = await result;
            ansind = 0;
            if (answer)
                ans[qno] = "Yes";
            else
                ans[qno] = "No";
            int c = 0, i = 0;
            string propertyValue = "";
            qno++;

            if (qno == 12)
            {
                ansind = 0;
                Methods rm = new Methods();
                JArray r = rm.getValues();
                foreach (JObject parsedObject in r.Children<JObject>())
                {
                    c = 0;
                    i = 0;
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        propertyValue = (string)parsedProperty.Value;
                        if (propertyValue.Equals(ans[i]))
                        {
                            c++;
                            i++;
                        }
                    }
                    if (c == 12)
                    {
                        try
                        {
                            finalans[k++] = propertyValue;
                            var options = new Guessed[] { Guessed.right, Guessed.wrong };
                            var descriptions = new string[] { "Am I Right", "Am I Wrong" };
                            PromptDialog.Choice<Guessed>(context, CheckAnswerAsync, options, "Is Your Character......." + finalans[ansind] + "", descriptions: descriptions);
                            // await context.PostAsync(finalans[ansind]);

                        }
                        catch (Exception e)
                        {
                            // await context.PostAsync("error------->"+e.ToString());
                        }
                    }
                }
                if (k == 0)
                {
                    await context.PostAsync("**No Such Animal or Bird Exists**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");
                }


            }
            else if (qno < 12)
            {
                var cardMsg = context.MakeMessage();
                var attachment = GetThumbnailCard(s[imgidx], "");
                cardMsg.Attachments.Add(attachment);
                await context.PostAsync(cardMsg);

                PromptDialog.Confirm(context, OnquestionReply, keys[qno], attachment.ToString());

            }
            else
                context.Wait(MessageReceived);


        }

        private async Task CheckAnswerAsync(IDialogContext context, IAwaitable<Guessed> result)
        {
            var selection = await result;

            ansind++;
            if (ansind < k+1)
            {
                if (selection.ToString().Equals("right"))
                {
                    await context.PostAsync("**Great, guessed right one more time !**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");
                }
                else if(ansind < k)
                {
                    try
                    {
                        var options = new Guessed[] { Guessed.right, Guessed.wrong };
                        var descriptions = new string[] { "Am I Right", "Am I Wrong" };
                        PromptDialog.Choice<Guessed>(context, CheckAnswerAsync, options, "Is Your Character......." + finalans[ansind], descriptions: descriptions);
                    }
                    catch (Exception e)
                    {

                    }

                }
                else
                {
                    await context.PostAsync("**No Such Animal or Bird Exists**");
                    PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");
                }
            }
            else
            {
                var cardMsg = context.MakeMessage();
                var attachment = bravo("", "");
                cardMsg.Attachments.Add(attachment);
                await context.PostAsync(cardMsg);

                await context.PostAsync("**Bravo!!!You Defeated Me**");
                PromptDialog.Confirm(context, startgame, "Do you wanna Play Again?");

            }

        }

        private Microsoft.Bot.Connector.Attachment BotWelcome(string responseFromQNAMaker, string userQuery)
        {
            var heroCard = new HeroCard
            {
                Title = userQuery,
                Subtitle = "",
                Text = responseFromQNAMaker,

                Images = new List<CardImage> { new CardImage("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\Welcome.gif") },

            };


            return heroCard.ToAttachment();
        }
        private Microsoft.Bot.Connector.Attachment bravo(string responseFromQNAMaker, string userQuery)
        {
            var heroCard = new HeroCard
            {
                Title = userQuery,
                Subtitle = "",
                Text = responseFromQNAMaker,

                Images = new List<CardImage> { new CardImage("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\bravo.gif") },

            };


            return heroCard.ToAttachment();
        }



        private Microsoft.Bot.Connector.Attachment GetThumbnailCard(string responseFromQNAMaker, string userQuery)
        {
            var heroCard = new ThumbnailCard
            {
                Title = responseFromQNAMaker,
                Images = new List<CardImage> { new CardImage("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\" + images[imgidx++]) },
            };
            return heroCard.ToAttachment();
        }
    }
}
