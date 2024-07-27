namespace UIFeatures.Inventory
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UserData.Model;

    public abstract class BaseStatItemView : TViewMono
    {
        [field: SerializeField] public Image ImgStatIcon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI  TxtStatName { get; private set; }

        public abstract void SetStatValue(StatDataElement statDataElement);
        
        public abstract void SetStatColor(Color color);
    }

    public class StatItemPresenter : BaseUIItemPresenter<BaseStatItemView, StatDataElement>
    {
        public StatItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(StatDataElement data)
        {
            this.View.TxtStatName.text = data.StatRecord.Name;
            this.View.SetStatValue(data);
            
            this.GetStatIcon(data.StatRecord.Icon).ContinueWith(sprite => this.View.ImgStatIcon.sprite = sprite);
            
            Color statColor = ColorUtility.TryParseHtmlString(data.StatRecord.ColorHex, out Color color) ? color : Color.white;
            
            this.View.ImgStatIcon.color = color;
            this.View.TxtStatName.color = color;
            this.View.SetStatColor(color);
        }

        private async UniTask<Sprite> GetStatIcon(string statIconPath) { return await this.GameAssets.LoadAssetAsync<Sprite>(statIconPath); }
    }
}