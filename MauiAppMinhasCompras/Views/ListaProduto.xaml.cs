using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing()
	{
        lista.Clear();
        List<Produto> tmp = await App.Db.GetAll();
        tmp.ForEach(i => lista.Add(i));

	}

	private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		}catch (Exception ex) { 
		
			DisplayAlert("Opss!", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
	{
		string q = e.NewTextValue;

		lista.Clear();

        List<Produto> tmp = await App.Db.Search(q);

        tmp.ForEach(i => lista.Add(i));
	}

	private void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}"; 

		DisplayAlert("Total dos Produtos", msg, "OK");

    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        if (sender is MenuItem menuItem)
        {
            if (menuItem.BindingContext is Produto produto)
            {
                bool confirmar = await DisplayAlert("Confirmar",
                    $"Remover '{produto.Descricao}'?", "Sim", "Não");

                if (confirmar)
                {
                    lista.Remove(produto);
                    await App.Db.Delete(produto.Id);
                }
            }
        }
     }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }

        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}