<template>
    <div v-show="isRepeating">
        <v-icon>event</v-icon>
        <label class="date-label">
            Datum im Serientermin
            <input v-if="focus" type="date" :value="value" @change="changeDateFrom" @blur="blurInput" ref="dateInput" autofocus>
            <template v-else>
                <input type="text" @focus="focusInput" style="width:0;height:0;">
                <span @click="focusInput" class="span-input">{{value | toLocalDate}}</span>
            </template>
        </label>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

@Component
export default class SimpleDatePicker extends Vue {
  @Prop(String) private value!: any
  private focus: boolean = false

  private changeDateFrom (e:any) {
    if (e.target.value) { this.value = e.target.value.substring(0, 10) }
  }
  private focusInput () {
    // @ts-ignore
    this.$nextTick(() => this.$refs.dateInput.focus())
    this.focus = true
  }
  private blurInput () {
    this.focus = false
  }
}
</script>
