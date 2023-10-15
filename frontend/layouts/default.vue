<template>
    <v-layout class="rounded rounded-md">
      <v-app-bar color="primary">
        <v-app-bar-title>
          <v-icon>mdi-thermometer</v-icon>サーバ室温湿度
        </v-app-bar-title>
          <template v-slot:append>

            <v-tooltip text="最新の計測">
              <template v-slot:activator="{ props }">
                <div class="d-flex justify-end ma-6" v-bind="props">
                      <v-icon start icon="mdi-new-box"></v-icon>
                      {{ moment(newestTemp?.measuredAt).format('YYYY-M-D HH:mm') }} ：{{ $formatUtility.round( newestTemp?.temperature, 1) }}℃ / {{ $formatUtility.round(newestTemp?.humidity, 1)}}% 
                </div>
              </template>
            </v-tooltip>

          </template>

      </v-app-bar>

      <v-main class="d-flex align-center justify-center" style="min-height: 300px;">
        <slot />
      </v-main>
    </v-layout>
</template>
<script setup lang="ts">
  import moment from 'moment';

  let lastFetch = ref()
  const {data: newestTemp} = useFetch(`/api/temperature/raspberrypi`,{
    watch: [lastFetch],
    onRequest:()=> {
      console.log("最新の温湿度を取得します... date=", lastFetch.value)
    }
  })
  
  let updateTimer: any

  onMounted(()=> {
		updateTimer = setInterval(async () => {
			lastFetch.value = new Date()
		}, 60* 5 *1000)
  })

  onUnmounted(()=>{
		clearInterval(updateTimer)
  })

</script>