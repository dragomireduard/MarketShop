using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Tema3Supermarket.Comands;
using Tema3Supermarket.Models;
using System.Data.Entity;

namespace Tema3Supermarket.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private const decimal AdaosComercial = 0.2m; // 20% adaos comercial

        private ObservableCollection<Produs> produse;
        public ObservableCollection<Produs> Produse
        {
            get { return produse; }
            set { produse = value; OnPropertyChanged("Produse"); }
        }

        private ObservableCollection<Categorie> categorii;
        public ObservableCollection<Categorie> Categorii
        {
            get { return categorii; }
            set { categorii = value; OnPropertyChanged("Categorii"); }
        }

        private ObservableCollection<Producator> producatori;
        public ObservableCollection<Producator> Producatori
        {
            get { return producatori; }
            set { producatori = value; OnPropertyChanged("Producatori"); }
        }

        private ObservableCollection<Stoc> stocuri;
        public ObservableCollection<Stoc> Stocuri
        {
            get { return stocuri; }
            set { stocuri = value; OnPropertyChanged("Stocuri"); }
        }

        private Categorie categorieSelectata;
        public Categorie CategorieSelectata
        {
            get { return categorieSelectata; }
            set { categorieSelectata = value; OnPropertyChanged("CategorieSelectata"); }
        }

        private Producator producatorSelectat;
        public Producator ProducatorSelectat
        {
            get { return producatorSelectat; }
            set { producatorSelectat = value; OnPropertyChanged("ProducatorSelectat"); }
        }

        private Produs produsSelectat;
        public Produs ProdusSelectat
        {
            get { return produsSelectat; }
            set { produsSelectat = value; OnPropertyChanged("ProdusSelectat"); }
        }

        private Stoc stocSelectat;
        public Stoc StocSelectat
        {
            get { return stocSelectat; }
            set { stocSelectat = value; OnPropertyChanged("StocSelectat"); }
        }

        private string numeProdus;
        public string NumeProdus
        {
            get { return numeProdus; }
            set { numeProdus = value; OnPropertyChanged("NumeProdus"); }
        }

        private string codBareProdus;
        public string CodBareProdus
        {
            get { return codBareProdus; }
            set { codBareProdus = value; OnPropertyChanged("CodBareProdus"); }
        }

        private int cantitateStoc;
        public int CantitateStoc
        {
            get { return cantitateStoc; }
            set { cantitateStoc = value; OnPropertyChanged("CantitateStoc"); }
        }

        private string unitateMasuraStoc;
        public string UnitateMasuraStoc
        {
            get { return unitateMasuraStoc; }
            set { unitateMasuraStoc = value; OnPropertyChanged("UnitateMasuraStoc"); }
        }

        private DateTime dataAprovizionareStoc = DateTime.Now;
        public DateTime DataAprovizionareStoc
        {
            get { return dataAprovizionareStoc; }
            set { dataAprovizionareStoc = value; OnPropertyChanged("DataAprovizionareStoc"); }
        }

        private DateTime dataExpirareStoc = DateTime.Now;
        public DateTime DataExpirareStoc
        {
            get { return dataExpirareStoc; }
            set { dataExpirareStoc = value; OnPropertyChanged("DataExpirareStoc"); }
        }

        private decimal pretAchizitieStoc;
        public decimal PretAchizitieStoc
        {
            get { return pretAchizitieStoc; }
            set
            {
                pretAchizitieStoc = value;
                OnPropertyChanged("PretAchizitieStoc");
                CalculeazaPretVanzare();
            }
        }

        private decimal pretVanzareStoc;
        public decimal PretVanzareStoc
        {
            get { return pretVanzareStoc; }
            set { pretVanzareStoc = value; OnPropertyChanged("PretVanzareStoc"); }
        }

        private ObservableCollection<dynamic> valoareCategorii;
        public ObservableCollection<dynamic> ValoareCategorii
        {
            get { return valoareCategorii; }
            set { valoareCategorii = value; OnPropertyChanged("ValoareCategorii"); }
        }

        public ICommand ListareProdusePeCategoriiCommand { get; set; }
        public ICommand CalculeazaValoareCategoriiCommand { get; set; }
        public ICommand AdaugaProdusCommand { get; set; }
        public ICommand ActualizeazaProdusCommand { get; set; }
        public ICommand StergeProdusCommand { get; set; }
        public ICommand AdaugaStocCommand { get; set; }
        public ICommand ActualizeazaStocCommand { get; set; }

        public ICommand VizualizareStocuriCommand { get; set; }

        public AdminViewModel()
        {
            Produse = new ObservableCollection<Produs>();
            Categorii = new ObservableCollection<Categorie>();
            Producatori = new ObservableCollection<Producator>();
            Stocuri = new ObservableCollection<Stoc>();
            ValoareCategorii = new ObservableCollection<dynamic>();
            LoadCategorii();
            LoadProducatori();
            LoadProduse();

            ListareProdusePeCategoriiCommand = new RelayCommand(param => ListareProdusePeCategorii());
            CalculeazaValoareCategoriiCommand = new RelayCommand(param => CalculeazaValoareCategorii());
            AdaugaProdusCommand = new RelayCommand(param => AdaugaProdus());
            ActualizeazaProdusCommand = new RelayCommand(param => ActualizeazaProdus());
            StergeProdusCommand = new RelayCommand(param => StergeProdus());
            AdaugaStocCommand = new RelayCommand(param => AdaugaStoc());
            ActualizeazaStocCommand = new RelayCommand(param => ActualizeazaStoc());
            VizualizareStocuriCommand = new RelayCommand(param => VizualizareStocuri());
        }
        private void VizualizareStocuri()
        {
            LoadStocuri();
        }
        private void LoadProduse()
        {
            using (var context = new SupermarketDbContext())
            {
                Produse = new ObservableCollection<Produs>(context.Produse.Include(p => p.Categorie).Include(p => p.Producator).ToList());
            }
        }

        private void LoadCategorii()
        {
            using (var context = new SupermarketDbContext())
            {
                Categorii = new ObservableCollection<Categorie>(context.Categorii.ToList());
            }
        }

        private void LoadProducatori()
        {
            using (var context = new SupermarketDbContext())
            {
                Producatori = new ObservableCollection<Producator>(context.Producatori.ToList());
            }
        }

        private void CalculeazaPretVanzare()
        {
            PretVanzareStoc = PretAchizitieStoc + (PretAchizitieStoc * AdaosComercial);
        }

        public void ListareProdusePeCategorii()
        {
            if (CategorieSelectata == null)
            {
                MessageBox.Show("Selectați o categorie.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                Produse = new ObservableCollection<Produs>(context.Produse.Include(p => p.Categorie).Include(p => p.Producator)
                    .Where(p => p.CategorieId == CategorieSelectata.Id).ToList());
            }
        }

        public void CalculeazaValoareCategorii()
        {
            using (var context = new SupermarketDbContext())
            {
                var categoriiValoare = context.Categorii.Include(c => c.Produse)
                    .Select(c => new
                    {
                        Categorie = c.Nume,
                        Valoare = c.Produse.Sum(p => p.Stocuri.Sum(s => s.PretVanzare * s.Cantitate))
                    }).ToList();

                ValoareCategorii = new ObservableCollection<dynamic>(categoriiValoare);
            }
        }

        private void AdaugaProdus()
        {
            if (CategorieSelectata == null || ProducatorSelectat == null)
            {
                MessageBox.Show("Selectați o categorie și un producător.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                // Verifică dacă categoria selectată există deja
                var categorieExistenta = context.Categorii.FirstOrDefault(c => c.Nume == CategorieSelectata.Nume);
                if (categorieExistenta != null)
                {
                    CategorieSelectata = categorieExistenta;
                }
                else
                {
                    context.Categorii.Add(CategorieSelectata);
                    context.SaveChanges();
                }

                // Verifică dacă producătorul selectat există deja
                var producatorExistent = context.Producatori.FirstOrDefault(p => p.Nume == ProducatorSelectat.Nume);
                if (producatorExistent != null)
                {
                    ProducatorSelectat = producatorExistent;
                }
                else
                {
                    context.Producatori.Add(ProducatorSelectat);
                    context.SaveChanges();
                }

                var produsNou = new Produs
                {
                    Nume = NumeProdus,
                    CodBare = CodBareProdus,
                    CategorieId = CategorieSelectata.Id,
                    ProducatorId = ProducatorSelectat.Id,
                    Categorie = CategorieSelectata, // Setarea referinței la Categorie
                    Producator = ProducatorSelectat  // Setarea referinței la Producator
                };

                context.Produse.Add(produsNou);
                context.SaveChanges();
                Produse.Add(produsNou);

                // Resetarea valorilor după adăugare
                NumeProdus = string.Empty;
                CodBareProdus = string.Empty;
                CategorieSelectata = null;
                ProducatorSelectat = null;
            }
        }

        private void ActualizeazaProdus()
        {
            if (ProdusSelectat == null)
            {
                MessageBox.Show("Selectați un produs.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                var produs = context.Produse.Find(ProdusSelectat.Id);
                if (produs != null)
                {
                    produs.Nume = ProdusSelectat.Nume;
                    produs.CodBare = ProdusSelectat.CodBare;
                    produs.CategorieId = ProdusSelectat.CategorieId;
                    produs.ProducatorId = ProdusSelectat.ProducatorId;
                    context.SaveChanges();
                    LoadProduse();
                }
            }
        }

        private void StergeProdus()
        {
            if (ProdusSelectat == null)
            {
                MessageBox.Show("Selectați un produs.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                var produs = context.Produse.Find(ProdusSelectat.Id);
                if (produs != null)
                {
                    context.Produse.Remove(produs);
                    context.SaveChanges();
                    Produse.Remove(ProdusSelectat);
                }
            }
        }

        private void AdaugaStoc()
        {
            if (ProdusSelectat == null)
            {
                MessageBox.Show("Selectați un produs.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                var stocNou = new Stoc
                {
                    ProdusId = ProdusSelectat.Id,
                    Cantitate = CantitateStoc,
                    UnitateMasura = UnitateMasuraStoc,
                    DataAprovizionare = DataAprovizionareStoc,
                    DataExpirare = DataExpirareStoc,
                    PretAchizitie = PretAchizitieStoc,
                    PretVanzare = PretVanzareStoc
                };
                context.Stocuri.Add(stocNou);
                context.SaveChanges();
                Stocuri.Add(stocNou);
                CantitateStoc = 0;
                UnitateMasuraStoc = string.Empty;
                DataAprovizionareStoc = DateTime.Now;
                DataExpirareStoc = DateTime.Now;
                PretAchizitieStoc = 0;
                PretVanzareStoc = 0;
            }
        }

        private void ActualizeazaStoc()
        {
            if (StocSelectat == null)
            {
                MessageBox.Show("Selectați un stoc.");
                return;
            }

            using (var context = new SupermarketDbContext())
            {
                var stoc = context.Stocuri.Find(StocSelectat.Id);
                if (stoc != null)
                {
                    stoc.Cantitate = CantitateStoc;
                    stoc.UnitateMasura = UnitateMasuraStoc;
                    stoc.DataAprovizionare = DataAprovizionareStoc;
                    stoc.DataExpirare = DataExpirareStoc;
                    stoc.PretAchizitie = PretAchizitieStoc;
                    stoc.PretVanzare = PretVanzareStoc;
                    context.SaveChanges();
                    LoadStocuri();
                }
            }
        }

        private void LoadStocuri()
        {
            using (var context = new SupermarketDbContext())
            {
                Stocuri = new ObservableCollection<Stoc>(context.Stocuri.Include(s => s.Produs).ToList());
            }
        }
    }
}
