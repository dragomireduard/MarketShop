using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Tema3Supermarket.Comands;
using Tema3Supermarket.Models;
using System.Data.Entity;
using System;
using System.Data.Entity.Validation;

namespace Tema3Supermarket.ViewModels
{
    public class CasierViewModel : BaseViewModel
    {
        private ObservableCollection<Produs> produse;
        public ObservableCollection<Produs> Produse
        {
            get { return produse; }
            set { produse = value; OnPropertyChanged("Produse"); }
        }

        private string criteriuCautare;
        public string CriteriuCautare
        {
            get { return criteriuCautare; }
            set { criteriuCautare = value; OnPropertyChanged("CriteriuCautare"); }
        }

        private Produs produsSelectat;
        public Produs ProdusSelectat
        {
            get { return produsSelectat; }
            set { produsSelectat = value; OnPropertyChanged("ProdusSelectat"); }
        }

        private ObservableCollection<ProdusBon> produseBon;
        public ObservableCollection<ProdusBon> ProduseBon
        {
            get { return produseBon; }
            set { produseBon = value; OnPropertyChanged("ProduseBon"); }
        }

        private decimal totalBon;
        public decimal TotalBon
        {
            get { return totalBon; }
            set { totalBon = value; OnPropertyChanged("TotalBon"); }
        }

        public ICommand AdaugaProdusPeBonCommand { get; set; }
        public ICommand EmitereBonCommand { get; set; }
        public ICommand CautareProduseCommand { get; set; }

        public CasierViewModel()
        {
            Produse = new ObservableCollection<Produs>();
            ProduseBon = new ObservableCollection<ProdusBon>();
            AdaugaProdusPeBonCommand = new RelayCommand(param => AdaugaProdusPeBon());
            EmitereBonCommand = new RelayCommand(param => EmitereBon());
            CautareProduseCommand = new RelayCommand(param => CautareProduse());
        }

        private void AdaugaProdusPeBon()
        {
            if (ProdusSelectat == null)
            {
                MessageBox.Show("Nu a fost selectat niciun produs.");
                return;
            }

            var stoc = ProdusSelectat.Stocuri.FirstOrDefault();
            if (stoc == null)
            {
                MessageBox.Show("Produsul selectat nu are stocuri disponibile.");
                return;
            }

            var produsBon = new ProdusBon
            {
                ProdusId = ProdusSelectat.Id,
                Cantitate = 1,  // Exemplu pentru o cantitate de 1
                Subtotal = stoc.PretVanzare,
                Produs = ProdusSelectat
            };
            ProduseBon.Add(produsBon);
            CalculeazaTotal();
        }


        private void CalculeazaTotal()
        {
            TotalBon = ProduseBon.Sum(pb => pb.Subtotal);
            MessageBox.Show($"Total Bon: {TotalBon}");
        }
        private void EmitereBon()
        {
            try
            {
                using (var context = new SupermarketDbContext())
                {
                    var bon = new BonCasa
                    {
                        DataEliberare = DateTime.Now,
                        CasierId = 1,  // Exemplu pentru casierul autenticat
                        SumaIncasata = TotalBon
                    };

                    // Adăugăm mesaj de diagnosticare înainte de salvare
                    MessageBox.Show("Adăugăm bonul în context.");

                    context.BonuriCasa.Add(bon);
                    context.SaveChanges();

                    // Adăugăm mesaj de diagnosticare după salvare
                    MessageBox.Show($"Bonul a fost salvat cu ID: {bon.Id}");

                    foreach (var produsBon in ProduseBon)
                    {
                        produsBon.BonId = bon.Id;
                        context.ProduseBon.Add(produsBon);
                    }

                    context.SaveChanges();

                    // Adăugăm mesaj de diagnosticare pentru confirmarea salvării produselor pe bon
                    MessageBox.Show("Produsele de pe bon au fost salvate.");

                    ProduseBon.Clear();
                    TotalBon = 0;
                    MessageBox.Show("Bonul a fost emis cu succes.");
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = $"Eroare la emiterea bonului: {fullErrorMessage}";
                MessageBox.Show(exceptionMessage);
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? "N/A";
                MessageBox.Show($"Eroare la emiterea bonului: {ex.Message}\nExcepție interioară: {innerExceptionMessage}\n{ex.StackTrace}");
            }
        }



        private void CautareProduse()
        {
            using (var context = new SupermarketDbContext())
            {
                var rezultate = context.Produse.Include(p => p.Categorie)
                                               .Include(p => p.Producator)
                                               .Include(p => p.Stocuri)
                                               .Where(p => p.Nume.Contains(CriteriuCautare) ||
                                                           p.CodBare.Contains(CriteriuCautare) ||
                                                           p.Producator.Nume.Contains(CriteriuCautare) ||
                                                           p.Categorie.Nume.Contains(CriteriuCautare))
                                               .ToList();
                Produse = new ObservableCollection<Produs>(rezultate);
            }
        }
    }
}
