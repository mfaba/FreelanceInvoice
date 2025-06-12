import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: true,
    imports: [CommonModule]
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
    public canvas: any;
    public ctx: any;
    public chartColor: string = "#FFFFFF";
    public chartEmail: any;
    public chartHours: any;
    public speedChart: any;

    ngOnInit() {
        // Initialization moved to ngAfterViewInit
    }

    ngAfterViewInit() {
        this.initCharts();
    }

    ngOnDestroy() {
        if (this.chartHours) {
            this.chartHours.destroy();
        }
        if (this.chartEmail) {
            this.chartEmail.destroy();
        }
        if (this.speedChart) {
            this.speedChart.destroy();
        }
    }

    private initCharts() {
        this.initHoursChart();
        this.initEmailChart();
        this.initSpeedChart();
    }

    private initHoursChart() {
        const canvas = document.getElementById("chartHours") as HTMLCanvasElement;
        if (!canvas) return;

        const ctx = canvas.getContext("2d");
        if (!ctx) return;

        this.chartHours = new Chart(ctx, {
            type: 'line',
            data: {
                labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct"],
                datasets: [{
                    borderColor: "#6bd098",
                    backgroundColor: "#6bd098",
                    pointRadius: 0,
                    pointHoverRadius: 0,
                    borderWidth: 3,
                    data: [300, 310, 316, 322, 330, 326, 333, 345, 338, 354]
                },
                {
                    borderColor: "#f17e5d",
                    backgroundColor: "#f17e5d",
                    pointRadius: 0,
                    pointHoverRadius: 0,
                    borderWidth: 3,
                    data: [320, 340, 365, 360, 370, 385, 390, 384, 408, 420]
                },
                {
                    borderColor: "#fcc468",
                    backgroundColor: "#fcc468",
                    pointRadius: 0,
                    pointHoverRadius: 0,
                    borderWidth: 3,
                    data: [370, 394, 415, 409, 425, 445, 460, 450, 478, 484]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        enabled: false
                    }
                },
                scales: {
                    y: {
                        ticks: {
                            color: "#9f9f9f",
                            maxTicksLimit: 5
                        },
                        grid: {
                            color: 'rgba(255,255,255,0.05)'
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(255,255,255,0.1)',
                            display: false
                        },
                        ticks: {
                            padding: 20,
                            color: "#9f9f9f"
                        }
                    }
                }
            }
        });
    }

    private initEmailChart() {
        const canvas = document.getElementById("chartEmail") as HTMLCanvasElement;
        if (!canvas) return;

        const ctx = canvas.getContext("2d");
        if (!ctx) return;

        this.chartEmail = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Opened', 'Read', 'Deleted', 'Unopened'],
                datasets: [{
                    label: "Emails",
                    backgroundColor: [
                        '#e3e3e3',
                        '#4acccd',
                        '#fcc468',
                        '#ef8157'
                    ],
                    borderWidth: 0,
                    data: [342, 480, 530, 120]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        enabled: false
                    }
                }
            }
        });
    }

    private initSpeedChart() {
        const canvas = document.getElementById("speedChart") as HTMLCanvasElement;
        if (!canvas) return;

        const ctx = canvas.getContext("2d");
        if (!ctx) return;

        this.speedChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                datasets: [{
                    borderColor: "#51CACF",
                    backgroundColor: "#51CACF",
                    pointRadius: 0,
                    pointHoverRadius: 0,
                    borderWidth: 3,
                    data: [600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150]
                },
                {
                    borderColor: "#EF7E56",
                    backgroundColor: "#EF7E56",
                    pointRadius: 0,
                    pointHoverRadius: 0,
                    borderWidth: 3,
                    data: [500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        enabled: false
                    }
                },
                scales: {
                    y: {
                        ticks: {
                            color: "#9f9f9f",
                            maxTicksLimit: 5
                        },
                        grid: {
                            color: 'rgba(255,255,255,0.05)'
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(255,255,255,0.1)',
                            display: false
                        },
                        ticks: {
                            padding: 20,
                            color: "#9f9f9f"
                        }
                    }
                }
            }
        });
    }
} 