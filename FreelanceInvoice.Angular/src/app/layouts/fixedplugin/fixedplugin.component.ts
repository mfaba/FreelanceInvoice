import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-fixed-plugin',
    templateUrl: './fixedplugin.component.html',
    styleUrls: ['./fixedplugin.component.scss'],
    standalone: true,
    imports: [CommonModule]
})
export class FixedPluginComponent implements OnInit {
    public sidebarColor: string = "white";
    public sidebarActiveColor: string = "danger";
    public state: boolean = true;

    changeSidebarColor(color: string) {
        const sidebar = document.querySelector('.sidebar') as HTMLElement;
        this.sidebarColor = color;
        if (sidebar) {
            sidebar.setAttribute('data-color', color);
        }
    }

    changeSidebarActiveColor(color: string) {
        const sidebar = document.querySelector('.sidebar') as HTMLElement;
        this.sidebarActiveColor = color;
        if (sidebar) {
            sidebar.setAttribute('data-active-color', color);
        }
    }

    ngOnInit() {}
} 