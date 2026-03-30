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
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Opss", ex.Message, "OK");
        }
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
        try
        {
            string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Opss", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
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
                var restante = await App.Db.GetAll();
                if (restante.Count == 0)
                {
                    await App.Db.ResetId();
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

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Opss", ex.Message, "OK");

        }finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        var produtos = await App.Db.GetAll();

        var relatorio = produtos
            .GroupBy(p => p.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                Total = g.Sum(p => p.Total)
            });

        string msg = "";

        foreach (var item in relatorio)
        {
            msg += $"{item.Categoria}: {item.Total:C}\n";
        }

        await DisplayAlert("Relatório por Categoria", msg, "OK");
    }

    private async void txt_categoria_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.FilterCategoria(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Opss", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

}