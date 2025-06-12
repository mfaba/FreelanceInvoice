import { Component } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { HeroComponent } from '../hero/hero';
import { AboutComponent } from '../about/about';
import { ServicesComponent } from '../services/services';
import { FeaturesComponent } from '../features/features';
import { FaqComponent } from '../faq/faq';
import { PricingComponent } from '../pricing/pricing';
import { BlogComponent } from '../blog/blog';
import { TestimonialComponent } from '../testimonial/testimonial';
 
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NavbarComponent,
    HeroComponent,
    AboutComponent,
    ServicesComponent,
    FeaturesComponent,
    FaqComponent,
    PricingComponent,
    BlogComponent,
    TestimonialComponent  ],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent {
}
