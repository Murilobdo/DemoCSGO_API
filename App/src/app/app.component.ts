import { Component, ElementRef, OnInit, TemplateRef, ViewChild  } from '@angular/core';
import { Player } from './models/Player';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

import { Chart, ChartType, ChartDataSets, RadialChartOptions } from 'chart.js';
import { Label } from 'ng2-charts';
import { StorageService } from './services/storage/storage.service';
import { Observable } from 'rxjs';

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

   // Radar
  public radarChartOptions: RadialChartOptions = {
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
    this.initPlayers();

  }


  public createChart(player: Player, template: TemplateRef<any>):void {
    this.radarChartData = [{
      data: [player.valAWPer, player.valLurker, player.valFragger, player.valSupport], label: player.nome
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
    this.downloadUrl.subscribe(data => this.storageService.SendURL(data));
  }

  initPlayers(): void{
    this.listPlayer.push({ id:1, nome:"Murilobdo", valAWPer:.3, valFragger:.51, valLurker:.22, valSupport:.15})
    this.listPlayer.push({ id:2, nome:"VitorFerreira", valAWPer:.7, valFragger:.57, valLurker:.38, valSupport:.50})
  }
}


