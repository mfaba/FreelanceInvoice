import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}

export const ROUTES: RouteInfo[] = [
    { path: '/dashboard',     title: 'Dashboard',         icon:'nc-bank',       class: '' },
    { path: '/invoices',      title: 'Invoices',          icon:'nc-paper',      class: '' },
    { path: '/clients',       title: 'Clients',           icon:'nc-single-02',  class: '' }
];

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
    standalone: true,
    imports: [CommonModule, RouterModule]
})
export class SidebarComponent implements OnInit {
    public menuItems: RouteInfo[] = [];
    
    ngOnInit() {
        this.menuItems = ROUTES.filter(menuItem => menuItem);
    }
} 