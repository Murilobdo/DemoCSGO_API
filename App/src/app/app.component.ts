import { Component, ElementRef, OnInit, TemplateRef, ViewChild  } from '@angular/core';
import { Player } from './models/Player';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

import { Chart, ChartType, ChartDataSets, RadialChartOptions, ChartFontOptions } from 'chart.js';
import { Label } from 'ng2-charts';
import { StorageService } from './services/storage/storage.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  @ViewChild('chart', { static: true })
  elemento!: ElementRef;
  public listPlayer!: Player[];
  public modalRef!: BsModalRef;
  public showUploadProgress!: boolean;
  public uploadProgress: Observable<number> = new Observable<number>();
  public downloadUrl!: Observable<string>;
  public messageResult!: string;
  public exibicao: number = 0;
   // Radar
  public radarChartOptions: RadialChartOptions = {
    legend: {
      display: true,
      labels: {
        fontSize: 30,
        fontColor: 'red',
      }
    },
    scale: {
      pointLabels: {
        fontSize: 20,
        fontStyle: 'bold',
        fontColor: 'black'
      }
    },
    responsive: true,
  };
  public radarChartData: ChartDataSets[] =[];
  public radarChartType: ChartType = 'radar';

  constructor(
    private modalService: BsModalService,
    private readonly storageService: StorageService
  ) {
    this.listPlayer = [];
    this.elemento = new ElementRef<any>(this.elemento);
  }



  ngOnInit(): void {
    //this.initPlayers();
    this.LoadResult();
  }


  public createChart(player: Player, template: TemplateRef<any>):void {
    this.radarChartData = [{
      data: [player.awper, player.lurker, player.entryFragger, player.suporte], label: player.name
    }]
    const config: ModalOptions = { class: 'modal-xl' };
    this.modalRef = this.modalService.show(template, config);

  }

  handleFileInput(event: any): void{
    var fileDEMO = event.target.files[0];
    const mediaFolderPath = '/demo';
    const { downloadUrl$, uploadProgress$ } = this.storageService.uploadFileAndGetMetadata(mediaFolderPath, fileDEMO);
    this.uploadProgress = uploadProgress$;
    this.showUploadProgress = true;
    this.downloadUrl = downloadUrl$;
    this.downloadUrl.subscribe(data => {
      this.exibicao = 1;
      this.storageService.SendURL(data)
        .subscribe(data => {
          console.log(data);
          this.messageResult = data;
          this.LoadResult();
          this.exibicao = 2;
        });
    });
  }

  public LoadResult(): any{
      this.storageService.GetJsonResult_1()
        .subscribe(data => {
          this.listPlayer = data;
          this.storageService.GetJsonResult_2()
          .subscribe(data => {
            //@ts-ignore
            data.forEach(element => {
              var player = this.listPlayer.find(p => p.name == element.Name);
              //@ts-ignore
              player.killed = element.Killed;
              //@ts-ignore
              player.death = element.Death;
              //@ts-ignore
              player.adr = element.ADR;
              //@ts-ignore
              player.clutches = element.Clutches;
              //@ts-ignore
              player.firstKills = element.FirstKills;
              //@ts-ignore
              player.blindedEnemies = element.FlashedEnemies;
            });

              console.table(this.listPlayer);
            })
        });
  }

  // initPlayers(): void{
  //   this.listPlayer.push({ id:1, name:"Murilobdo", valAWPer:.3, valFragger:.51, valLurker:.22, valSupport:.15})
  //   this.listPlayer.push({ id:2, nome:"VitorFerreira", valAWPer:.7, valFragger:.57, valLurker:.38, valSupport:.50})
  // }
}


