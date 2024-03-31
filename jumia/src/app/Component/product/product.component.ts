import { Component, OnInit, SimpleChanges } from '@angular/core';
import { FilterComponent } from "../filter/filter.component";
import { ApiProductsService } from '../../Services/api-products.service';
import { ProductDto } from '../../ViewModels/product-dto';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { SearchResultsService } from '../../Services/search-results.service';


@Component({
    selector: 'app-product',
    standalone: true,
    templateUrl: './product.component.html',
    styleUrl: './product.component.css',
    imports: [FilterComponent,CommonModule]
})
export class ProductComponent implements OnInit{
  productList: ProductDto[] = [];
   AllProducts:ProductDto[]=[];
   CountProducts:number=0;
    products: any;
    searchResults: any[] = [];
    pageSize:number = 15; 
    AllProd:number=0;
  totalPages: number = 0; 
  pageNumber: number = 1; 
  pageNumbers: number[]=[];
 // Method to calculate pages array



    constructor(private _ApiProductsService :ApiProductsService ,private _searchResultsService: SearchResultsService, private _sanitizer:DomSanitizer) { }
    
    
   
    ngOnInit(): void {
      
      
      this.Sershresult();
      this.getAllProducts();
      }


      Sershresult() {
        this._searchResultsService.getSearchResults().subscribe({
            next: (data) => {
                this.AllProducts = data;
                console.log(this.AllProducts );
                this.sanitizeImages();
            
            },
            error: (err) => {
                console.log(err);
            }
        });
    }
 
      getAllProducts() {
        this._ApiProductsService.getAllProducts(this.pageSize, this.pageNumber).subscribe({
            next: (data) => {
                this.AllProducts = data.entities;
                this.totalPages=Math.ceil( data.count/ this.pageSize)
                this.pageNumbers = Array.from({ length: this.totalPages }, (_, index) => index + 1);
                
                
                this.sanitizeImages();
            },
            error: (err) => {
                console.log(err);
            }
        });
    }
    nextPage(): void {
      if (this.pageNumber < this.totalPages) {
        console.log( this.pageNumber);
        
        this.pageNumber++;
        console.log( this.pageNumber);
        

console.log();
        this.getAllProducts();
      }
    }
  
    prevPage(): void {
      if (this.pageNumber > 1) {
        this.pageNumber--;
        this.getAllProducts();
      }
    }
  
    goToPage(page: number): void {
      if (page >= 1 && page <= this.totalPages) {
        this.pageNumber = page;
        this.getAllProducts();
      }
    }


      // getAllProducts() {
      //   this._ApiProductsService.getAllProducts().subscribe({
      //     next: (data) => {
      //       this.AllProducts = data;
      //       this.sanitizeImages();
      //     },
      //     error: (err) => {
      //       console.log(err);
      //     }
      //   });
      // }
      loadAllProductsOrderedAsc() {
        this._ApiProductsService.getAllProductsWithOrderAasc().subscribe({
          next: (data) => {
            this.AllProducts = data;
            this.AllProd=data.length;
            this.sanitizeImages();
          },
          error: (err) => {
            console.log(err);
          }
        });
      }
    
      loadAllProductsOrderedDsc() {
        this._ApiProductsService.getAllProductsWithOrderDasc().subscribe({
          next: (data) => {
            this.AllProducts = data;
            this.sanitizeImages();
          },
          error: (err) => {
            console.log(err);
          }
        });
      }
    
      loadAllProductsNewestArrivals() {
        this._ApiProductsService.getAllProductsWithNewestArrivals().subscribe({
          next: (data) => {
            this.AllProducts = data;
            this.sanitizeImages();
          },
          error: (err) => {
            console.log(err);
          }
        });
      }
    
      sanitizeImages() {
        this.AllProducts.forEach(product => {
          product.images = this._sanitizer.bypassSecurityTrustUrl('data:image/jpeg;base64,' + product.images);
        });
      }
    
      onSortChange(event: any) {
        const selectedSortOption = event.target.value;
        switch (selectedSortOption) {
          case 'allProducts':
            this.getAllProducts();
            break;
          case 'PriceLowToHigh':
            this.loadAllProductsOrderedAsc();
            break;
          case 'PriceHighToLow':
            this.loadAllProductsOrderedDsc();
            break;
          case 'NewestArrivals':
            this.loadAllProductsNewestArrivals();
            break;
          default:
            this.getAllProducts();
            break;
        }
      }


      searchProducts(searchTerm: string): void {
        this._ApiProductsService.SearchByNameOrDesc(searchTerm).subscribe(
          (data: any) => {
            this.searchResults = data;
            console.log(data)
          },
          (error: any) => {
            console.error('Error fetching search results:', error);
          }
        );
        }

    // ngOnInit(): void {
    //     this._ApiProductsService.getAllProductsWithOrderAasc().subscribe({
    //         next:(data)=>{
    //       this.AllProducts=data
          
    //       this.AllProducts.forEach(Product => {
            
    //         Product.images = this._sanitizer.bypassSecurityTrustUrl('data:image/jpeg;base64,' + Product.images);
    //      });

    //       },
    //       error:(err)=>{
          
    //       console.log(err)
    //       }
          
    //       })
    // } 
  
}
