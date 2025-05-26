using System.Text.RegularExpressions;

namespace CoreLib.DevTools.Extensions
{
    public static class ItemTypeExtensions
    {
        public static string TranslateItemType(this ItemType itemType, bool useColors = false)
        {
            string ret = itemType switch
            {
                ItemType.KeycardJanitor => "<color=#b9acde>КК Уборщика</color>",
                ItemType.KeycardScientist => "<color=#e7d573>КК Учёного</color>",
                ItemType.KeycardResearchCoordinator => "<color=#deae21>КК Глав. Ученого</color>",
                ItemType.KeycardZoneManager => "<color=#217777>КК Менеджера зоны</color>",
                ItemType.KeycardGuard => "<color=#55606c>КК Охранника</color>",
                ItemType.KeycardMTFPrivate => "<color=#a3c9db>КК Кадета</color>",
                ItemType.KeycardContainmentEngineer => "<color=#bd8e84>КК Инженера</color>",
                ItemType.KeycardMTFOperative => "<color=#466fd4>КК Лейтенанта</color>",
                ItemType.KeycardMTFCaptain => "<color=#224ad0>КК Капитана</color>",
                ItemType.KeycardFacilityManager => "<color=#af1a43>КК Менеджера комплекса</color>",
                ItemType.KeycardChaosInsurgency => "<color=#008f1c>КК Повстанцев хаоса</color>",
                ItemType.KeycardO5 => "<color=#212021>КК Совета O5</color>",
                ItemType.Radio => "<color=#515c41>Рация</color>",
                ItemType.GunCOM15 => "<color=#474444>COM15</color>",
                ItemType.Medkit => "<color=#b22f21>Аптечка</color>",
                ItemType.Flashlight => "<color=#c1ccce>Фонарик</color>",
                ItemType.MicroHID => "<color=#206288>MicroHID</color>",
                ItemType.SCP500 => "<color=#841421>SCP500-Панацея</color>",
                ItemType.SCP207 => "<color=#ad2021>SCP207-Кола</color>",
                ItemType.Ammo12gauge => "<color=#202020>Патроны 12х72</color>",
                ItemType.GunE11SR => "<color=#5b3b3d>E11SR</color>",
                ItemType.GunCrossvec => "<color=#5c6197>Crossvec</color>",
                ItemType.Ammo556x45 => "<color=#202020>Патроны 5.56х45</color>",
                ItemType.GunFSP9 => "<color=#375e5b>FSP9</color>",
                ItemType.GunLogicer => "<color=#887751>Logicer</color>",
                ItemType.GrenadeHE => "<color=#5d7b99>Осколочная граната</color>",
                ItemType.GrenadeFlash => "<color=#bcc4b6>Светошумовая граната</color>",
                ItemType.Ammo44cal => "<color=#202020>Патоны .44</color>",
                ItemType.Ammo762x39 => "<color=#202020>Патроны 7.62x39</color>",
                ItemType.Ammo9x19 => "<color=#202020>Патроны 9x19</color>",
                ItemType.GunCOM18 => "<color=#96877b>COM18</color>",
                ItemType.SCP018 => "<color=#5a0408>SCP018-Мячик</color>",
                ItemType.SCP268 => "<color=#635338>SCP268-Кепка</color>",
                ItemType.Adrenaline => "<color=#95281c>Адреналин</color>",
                ItemType.Painkillers => "<color=#c46a3f>Обез</color><color=#a19f98>болива</color><color=#3e6a75>ющее</color>",
                ItemType.Coin => "<color=#827572>Монетка</color>",
                ItemType.ArmorLight => "<color=#585151>Легкая броня</color>",
                ItemType.ArmorCombat => "<color=#686558>Средняя броня</color>",
                ItemType.ArmorHeavy => "<color=#716943>Тяжелая броня</color>",
                ItemType.GunRevolver => "<color=#72716b>Револьвер</color>",
                ItemType.GunAK => "<color=#636363>AK</color>",
                ItemType.GunShotgun => "<color=#ab8a8a>Дробовик</color>",
                ItemType.SCP330 => "<color=#212e47>SCP330-Конфеты</color>",
                ItemType.SCP2176 => "<color=#4e7354>SCP2176-Лампочка</color>",
                ItemType.SCP244a => "<color=#ab9c8a>SCP244a-Ваза</color>",
                ItemType.SCP244b => "<color=#5d7686>SCP244b-Ваза</color>",
                ItemType.SCP1853 => "<color=#3f692b>SCP1853-Допинг</color>",
                ItemType.ParticleDisruptor => "<color=#4c61b5>Разрушитель частиц</color>",
                ItemType.GunCom45 => "<color=#636363>Com45</color>",
                ItemType.SCP1576 => "<color=#c1b073>SCP1576-Громофон</color>",
                ItemType.Jailbird => "<color=#93c4c9>Jailbird-Палка</color>",
                ItemType.AntiSCP207 => "<color=#9e7167>Анти-кола</color>",
                ItemType.GunFRMG0 => "<color=#636363>FR-MG-0</color>",
                ItemType.GunA7 => "<color=#3c2b28>A</color><color=#636363>7</color>",
                ItemType.Lantern => "<color=#e1b996>Лампа</color>",
                ItemType.None => "<color=#ffffff>Ничто</color>",
                _ => itemType.ToString()
            };
            return useColors ? ret : Regex.Replace(ret, "<[^<>]*>", "");
        }
    }
}