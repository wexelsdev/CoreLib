using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CoreLib
{
    public class Translations : ITranslation
    {
        //AntiAFK
        [Description("Field to replace: [AFK_TIME], [MAX_AFK_TIME] (Configurable in config)")]
        public string AfkMessage { get; private set; } = "<b>Выйдите из АФК, иначе вы будете отправлены в наблюдатели. | [AFK_TIME]/[MAX_AFK_TIME]</b>";
        public string MessageNewPlayer { get; private set; } = "<b>Вы заспавнены за АФК игрока.\nЕсли что-то не так, то напишите .kill или обратитесь к администратору.</b>";
    }
}