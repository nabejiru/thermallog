export interface FormatUtilityInterface {
    round (val:number, digits:number): number
  }
  
  class FormatUtility implements FormatUtilityInterface {
    round (val:number, digits:number): number {
        const x = Math.pow(10, digits)
      return Math.round(val * x) / x;
    }
  }
  
  export default defineNuxtPlugin(nuxtApp => {
    return {
      provide: {
        formatUtility: new FormatUtility
      }
    }
  })
  