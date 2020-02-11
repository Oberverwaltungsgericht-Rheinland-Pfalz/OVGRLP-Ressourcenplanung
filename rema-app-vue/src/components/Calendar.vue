<template>
  <v-layout fill-height>
    <v-flex>
      <v-sheet height="64">
        <v-toolbar flat color="white">
          <v-btn outlined class="mr-4" @click="setToday">Heute</v-btn>
          <v-btn fab text small @click="prev">
            <v-icon small>arrow_back_ios</v-icon>
          </v-btn>
          <v-btn fab text small @click="next">
            <v-icon small>arrow_forward_ios</v-icon>
          </v-btn>
          <v-toolbar-title>{{ title }}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-switch v-model="viewType" inset :label="typeToLabel[type]"></v-switch>
        </v-toolbar>
      </v-sheet>

      <v-sheet height="600">
        <v-calendar
          ref="calendar"
          v-model="focus"
          color="primary"
          :events="itemsFormated"
          :event-color="getEventColor"
          :event-margin-bottom="3"
          :now="today"
          :type="type"
          :weekdays="[1, 2, 3, 4, 5, 6, 0]"
          :short-weekdays="false"
          @click:event="showEvent"
          @click:more="viewDay"
          @click:date="viewDay"
          @change="updateRange"
        ></v-calendar>
        <v-menu
          v-model="selectedOpen"
          :close-on-content-click="false"
          :activator="selectedElement"
          offset-x
        >
          <v-card color="grey lighten-4" min-width="350px" flat>
            <v-toolbar :color="selectedEvent.color" dark>
              <!--<v-btn icon>
                <v-icon>edit</v-icon>
              </v-btn>-->
              <!--<v-btn icon>
                <v-icon>favorite</v-icon>
              </v-btn>
              <v-btn icon>
                <v-icon>more_vert</v-icon>
              </v-btn>-->
              <v-toolbar-title v-html="selectedEvent.name"></v-toolbar-title>
              <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
              <span v-html="selectedEvent.details"></span>
            </v-card-text>
            <v-card-actions>
              <v-btn text color="secondary" @click="selectedOpen = false">Schließen</v-btn>
            </v-card-actions>
          </v-card>
        </v-menu>
      </v-sheet>
    </v-flex>
  </v-layout>
</template>

<script>
import dayjs from 'dayjs'
import { Allocation } from '../models'
var moment = require('moment')

export default {
  data: () => ({
    viewType: true,
    today: dayjs().format('YYYY-MM-DD'),
    focus: dayjs().format('YYYY-MM-DD'),
    typeToLabel: {
      month: 'Monat',
      week: 'Woche',
      day: 'Tag',
      '4day': '4 Tage'
    },
    start: null,
    end: null,
    selectedEvent: {},
    selectedElement: null,
    selectedOpen: false
    /*    events: [
      {
        name: 'IT Meeting',
        details: 'Spending time on how we do not have enough time',
        start: `${dayjs().format('YYYY-MM-DD')} 09:00`,
        end: `${dayjs().format('YYYY-MM-DD')} 19:00`,
        color: 'indigo'
      }
    ] */
  }),
  computed: {
    type: {
      get () {
        return this.viewType ? 'month' : 'day'
      },
      set (v) {
        this.viewType = v
      }
    },
    items () {
      return Allocation.query()
        .with('Purpose')
        .with('Ressource')
        .get()
    },
    itemsFormated () {
      return this.items.map(transfer2Calendar)
    },
    title () {
      const { start, end } = this
      if (!start || !end) return dayjs().format('MMMM YYYY')

      const startMonth = this.monthFormatter(start)
      const endMonth = this.monthFormatter(end)
      const suffixMonth = startMonth === endMonth ? '' : endMonth

      const startYear = start.year
      const endYear = end.year
      const suffixYear = startYear === endYear ? '' : endYear

      const startDay = start.day + this.nth(start.day)
      const endDay = end.day + this.nth(end.day)

      switch (this.type) {
        case 'month':
          return `${startMonth} ${startYear}`
        case 'week':
        case '4day':
          return `${startMonth} ${startDay} ${startYear} - ${suffixMonth} ${endDay} ${suffixYear}`
        case 'day':
          return `${startMonth} ${startDay} ${startYear}`
      }
      return ''
    },
    monthFormatter () {
      return this.$refs.calendar.getFormatter({
        timeZone: 'UTC',
        month: 'long'
      })
    }
  },
  methods: {
    viewDay ({ date }) {
      this.focus = date
      this.type = 'day'
    },
    getEventColor (event) {
      return event.color
    },
    setToday () {
      this.focus = this.today
    },
    prev () {
      this.$refs.calendar.prev()
    },
    next () {
      this.$refs.calendar.next()
    },
    showEvent ({ nativeEvent, event }) {
      const open = () => {
        this.selectedEvent = event
        this.selectedElement = nativeEvent.target
        setTimeout(() => (this.selectedOpen = true), 10)
      }

      if (this.selectedOpen) {
        this.selectedOpen = false
        setTimeout(open, 10)
      } else {
        open()
      }

      nativeEvent.stopPropagation()
    },
    updateRange ({ start, end }) {
      // You could load events from an outside source (like database)
      // now that we have the start and end dates on the calendar
      this.start = start
      this.end = end
    },
    nth (d) {
      return d > 3 && d < 21
        ? 'th'
        : ['th', 'st', 'nd', 'rd', 'th', 'th', 'th', 'th', 'th', 'th'][d % 10]
    }
  }
}
function transfer2Calendar (v) {
  let rVal = {}
  if (v.IsAllDay) {
    rVal.start = v.From.substring(0, 10)
    rVal.details = `ganztägig ${
      (v.Purpose || {}).Notes
    } || ''`
  } else {
    rVal.start = v.From.substring(0, 16).replace('T', ' ') // dayjs(v.From).format('YYYY-MM-DD hh:mm'),
    rVal.end = v.To.substring(0, 16).replace('T', ' ') // dayjs(v.To).format('YYYY-MM-DD hh:mm'),
    rVal.details = `
      von ${moment(v.From).format('LT')} 
      bis ${moment(v.To).format('LT')}
    `
  }
  rVal.name = v.Title + ' in ' + (v.Ressource || {}).Name
  rVal.color = 'success'
  rVal.id = v.Id

  return rVal
}
</script>
