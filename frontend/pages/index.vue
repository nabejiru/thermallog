<template>
  <v-container>
    <v-row no-gutters >
      <v-col cols="12" md="4">
        <v-select label="対象年" :items="years" class="w-25" variant="outlined" density="compact" v-model="selectedYear"></v-select>
      </v-col>
    </v-row>
      <v-row no-gutters >
        <v-col cols="12" md="6">
          <v-card class="ma-2" :title="`月ごとの推移（${selectedYear} 年）`">
            <v-card-item>
              <Line ref="monthlyChart" :data="monthlyData" :options="monthlyOption" />
            </v-card-item>
          </v-card>
        </v-col> 
        <v-col cols="12" md="6">
          <v-card class="ma-2"  :title="`日ごとの推移（${selectedYear} 年 ${selectedMonth} 月）`">
            <v-card-item>
              <Line ref="dailyChart" :data="dailyData" :options="dailyOption" />
            </v-card-item>
          </v-card>
        </v-col> 

        <v-col cols="12" md="6">
          <v-card  class="ma-2" :title="`時間ごとの推移（${ selectedYear }年 ${ selectedMonth }月 ${ selectedDay }日）`">
            <v-card-item>
              <Line ref="hourlyChart" :data="hourlyData" :options="hourlyOption" /> 
            </v-card-item>
          </v-card>
        </v-col>

      </v-row>
    <v-overlay v-model="progress" class="d-flex align-center justify-center">
        <v-progress-circular
        size="100"
        color="primary"
        indeterminate
      ></v-progress-circular>
    </v-overlay>
  
  </v-container>
</template>

<script lang="ts" setup>
import { ref, watch, computed } from 'vue'
import { Line } from 'vue-chartjs'
import moment from 'moment';

const { $formatUtility } = useNuxtApp()
const hostName = "raspberrypi"
const {data: years} = await useFetch('/api/available_years', {
  query: {hostName : hostName}
})

const selectedYear = ref<number>(moment().year());
const selectedMonth = ref(moment().month()+1);
const selectedDay = ref(moment().date());
const progress = computed(()=> pendingMonthly.value && pendingDaily.value && pendingHourly.value)

if(years.value && Array.isArray(years.value) ) {
  selectedYear.value = Math.max(...years.value);
} else {
  years.value = [""]
}

const {data: monthlyTemps, pending: pendingMonthly } = useFetch(`/api/temperatures/${hostName}`, {
  query: { y: selectedYear},
  watch: [selectedYear]
})
const {data: dailyTemps, pending : pendingDaily } = useFetch(`/api/temperatures/${hostName}`, {
  query: { y: selectedYear, m: selectedMonth },
  watch: [selectedYear, selectedMonth]
})
const {data: hourlyTemps, pending : pendingHourly } = useFetch(`/api/temperatures/${hostName}`, {
  query: { y: selectedYear, m: selectedMonth, d: selectedDay },
  watch: [selectedYear, selectedMonth, selectedDay]
})

const createDatasets = (tempretures, xAxis) => {
  return [
        {
          label: '最高気温(℃)',
          data: tempretures?.map((t)=> { return {x:xAxis(t.date), y:t.maxTemperature } }),
          radius: 0,
          borderDash: [3,3],
          borderColor: "#ff9900",
          backgroundColor: "#ff990044",
          borderWidth: 1,
          yAxisID: "y"
        },
        {
          label: '最低気温(℃)',
          data: tempretures?.map((t)=> { return {x:xAxis(t.date), y:t.minTemperature } }),
          radius: 0,
          borderDash: [3,3],
          borderColor: "#ff9900",
          backgroundColor: "#ff990044",
          fill: '-1',
          borderWidth: 1,
          yAxisID: "y"
        },
        {
          label: '平均気温(℃)',
          data: tempretures?.map((t)=> { return {x:xAxis(t.date), y:t.avgTemperature } }),
          radius: 0,
          borderColor: "#ff9900",
          backgroundColor: "#ff990044",
          borderWidth: 1,
          yAxisID: "y"
        },
        {
          label: '最高湿度(%)',
          data: tempretures?.map((t)=>  { return {x:xAxis(t.date), y:t.maxHumidity } }),
          borderDash: [3,3],
          borderColor: "#0099ff",
          backgroundColor: "#0099ff44",
          borderWidth: 1,
          radius: 0,
          yAxisID: "y1"
        },
        {
          label: '最低湿度(%)',
          data: tempretures?.map((t)=>  { return {x:xAxis(t.date), y:t.minHumidity } }),
          borderColor: "#0099ff",
          borderDash: [3,3],
          backgroundColor: "#0099ff44",
          borderWidth: 1,
          fill: '-1',
          radius: 0,
          yAxisID: "y1"
        },
        {
          label: '平均湿度(%)',
          data: tempretures?.map((t)=>  { return {x:xAxis(t.date), y:t.avgHumidity } }),
          borderColor: "#0099ff",
          backgroundColor: "#0099ff44",
          borderWidth: 1,
          radius: 0,
          yAxisID: "y1"
        }
  ]
}

const dailyData = computed(()=>{
  return {
  labels : [...Array(31).keys()].map((m)=> `${m+1}日` ),
  datasets : createDatasets(dailyTemps.value, (date)=> moment(date).format('D日'))
  }
})
const monthlyData = computed (()=>{
  console.debug(monthlyTemps.value);
  return {
    
    labels : [...Array(12).keys()].map((m)=> moment(`${selectedYear.value}-${m+1}`).format('YYYY-MM') ),
    datasets : createDatasets(monthlyTemps.value, (date)=> moment(date).format('YYYY-MM'))
  }
})
const hourlyData = computed(()=>{
  return {
    labels : [...Array(24).keys()].map((h)=> `${h}時` ),
    datasets : createDatasets(hourlyTemps.value, (date)=>moment(date).format('H時')) 
  }
})

const TEMP_THRESHOLD = 27
const HUMI_THRESHOLD = 50

const baseOptions = {
    responsive: true,
    interaction: {
      intersect: false,
      mode: 'index',
    },
    animations: {
      numbers: { duration: 0 },
      colors: {
        type: "color",
        duration: 2000,
        from: "transparent",
      }
    },
    plugins: {
      filler: {
        propagate: false
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        enabled: true,
      },
      // 基準線
      annotation: {
        annotations: {
          temp_line: {
            type: 'line',
            scaleID: 'y',
            value: TEMP_THRESHOLD,
            borderColor: '#ff0000',
            borderWidth: 1,
          },
          humi_line: {
            type: 'line',
            scaleID: 'y1',
            value: HUMI_THRESHOLD,
            borderColor: '#0000ff',
            borderWidth: 1,
            label: {
              enabled: true,
              content: '目標'
            }

          },
        }
      },
    },
    events: ['click'],
    scales: {
      x: {
      },
      y: {
        title: {
          display: true,
          text: '室温(℃)'
        },
        min: -50,
        max: 50,
        ticks: {
          stepSize: 10
        }
      },
      y1: {
        title: {
          display: true,
          text: '湿度(%)'
        },
        min: 0,
        max: 100,
        position: "right",
        ticks: {
          stepSize: 10
        }
      }
    }
  };
  const monthlyOption = ref({...baseOptions, 
    ...{
      onClick: (event, elements, chart) => {
        if (elements[0]) {            
          const i = elements[0].index;
          let temp = monthlyTemps.value[i]
          selectedMonth.value = moment(temp.date).month()+1;
        }
      }
    }
  })
  const dailyOption = ref({...baseOptions, 
    ...{
      onClick: (event, elements, chart) => {
        if (elements[0]) {            
          const i = elements[0].index;
          let temp = dailyTemps.value[i]
          selectedDay.value = moment(temp.date, 'YYYY-MM-DD').date();
        }
      }
    }
  })
  const hourlyOption = ref({...baseOptions, 
    ...{
    }
  })

</script>