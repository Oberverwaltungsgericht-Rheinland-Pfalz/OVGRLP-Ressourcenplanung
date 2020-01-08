<template>
  <div class="ressource-scheduler">
    <div id="chart" ref="chartdiv"></div>
    <b>TIMELINE:</b> MOVE
    <v-btn @click="moveAxisX('left')">LEFT</v-btn>
    <v-btn @click="moveAxisX('right')">RIGHT</v-btn>| ZOOM IN/OUT
    <v-btn @click="zoomAxisX('in')">IN</v-btn>
    <v-btn @click="zoomAxisX('out')">OUT</v-btn>
    <v-btn @click="zoomViewAllAxisX()">VIEW ALL</v-btn>| Or use
    <b>click+move</b> & <b>mousewheel</b> directly on graph.
    <hr />
    <div v-show="false">
      <b>START DATE ALL:</b> MOVE
      <v-btn @click="moveStartDate(-10 * 60)">-10 MIN</v-btn>
      <v-btn @click="moveStartDate(10 * 60)">+10 MIN</v-btn>| current =
      <span>{{ dateStartAll || formatDate }}</span>
      <hr />
      <b>SLOT:</b>
      <v-btn @click="buildAutoSlot()">BUILD AUTO</v-btn>|
      <v-btn @click="removeAllSlot()">REMOVE ALL</v-btn>Or
      <b>double click</b>
      on slot to remove it. | ADD
      <!--    <v-btn @click='addSlot ('A')'>A</v-btn><v-btn @click='addSlot ('B')'>B</v-btn><v-btn @click='addSlot ('C')'>C</v-btn><v-btn @click='addSlot ('D')'>D</v-btn><v-btn @click='addSlot ('E')'>E</v-btn><v-btn @click='addSlot ('F')'>F</v-btn><v-btn @click='addSlot ('X')'>X</v-btn>
      -->
      <hr />
    </div>
    <div id="selection"></div>
  </div>
</template>

<script>
// includes code from d3-gantt-scheduler, @Repository: https://github.com/bertrandg/d3-gantt-scheduler/, @by Author 'bertrandg <bertrandgaillard@hotmail.fr>  (https://github.com/bertrandg)', @Licensed under ISC License
import _ from 'lodash'
import * as d3 from 'd3'
import Ressources from '../models/RessourceModel'
import Allocations from '../models/AllocationModel'
import dayjs from 'dayjs'

export default {
  data () {
    return {
      scaleX: null,
      scaleY: null,
      axisX: null,
      axisY: null,
      WIDTH: 0,
      HEIGHT: 500,
      BORDER: 50,
      GAP: 60,
      svg: null,
      currentSlot: null,
      elMain: null,
      dateStartAll: new Date(),
      elContainer: null,
      elAxisX: null,
      elAxisY: null,
      startDate: Date.now(),
      data: [{ id: 3, name: 'Raum 2', startAfter: 4 * 60, duration: 24 * 36e2 }]
    }
  },
  computed: {
    data2 () {
      let roomNames = Allocations.query()
        .with('Ressource')
        .get()
      let blocks = roomNames.map(v => {
        const rValue = {
          id: v.Id,
          name: v.Ressource.Name,
          from: v.From,
          to: v.To
        }
        const today = dayjs(new Date())
        const start = dayjs(v.From)
        const diff = start.diff(today, 'hour') // 3600
        const durationTime = dayjs(v.To).diff(start, 'minute') // 60

        rValue.startAfter = diff * 3600
        if (v.IsAllDay) rValue.duration = 864e2
        else rValue.duration = durationTime * 60

        return rValue
      })
      return blocks
    }
  },
  methods: {
    moveAxisX (direction) {
      const [start, end] = this.scaleX.domain()
      const newStart = this.scaleX.invert(this.WIDTH * 0.2)
      let duration = getDurationBetween(start, newStart)
      duration = direction === 'left' ? -duration : duration

      this.updateAxisX(
        getDatePlusDuration(start, duration),
        getDatePlusDuration(end, duration)
      )
    },
    zoomAxisX (dir) {
      const [start, end] = this.scaleX.domain()
      const durationVisible = getDurationBetween(start, end)

      let diff = 2 * 60
      switch (true) {
        // less 15min visible > move 1min each side if unzoom
        case durationVisible <= 10 * 60:
          diff = dir === 'out' ? 1 * 60 : 0
          break
        // less 30min visible > move 1min each side
        case durationVisible <= 30 * 60:
          diff = 1 * 60
          break
        // less 1hour visible > move 4min each side
        case durationVisible <= 60 * 60:
          diff = 4 * 60
          break
        // less 3hour visible > move 8min each side
        case durationVisible <= 3 * 60 * 60:
          diff = 8 * 60
          break
        // less 6hour visible > move 12min each side
        case durationVisible <= 6 * 60 * 60:
          diff = 12 * 60
          break
      }

      if (dir === 'in') {
        this.updateAxisX(
          getDatePlusDuration(start, diff),
          getDatePlusDuration(end, -diff)
        )
      } else if (dir === 'out') {
        this.updateAxisX(
          getDatePlusDuration(start, -diff),
          getDatePlusDuration(end, diff)
        )
      }
    },
    zoomViewAllAxisX (animDuration = 200) {
      const startMin = _.min(_.map(this.data, d => this.getStart(d).getTime()))
      const endMax = _.max(_.map(this.data, d => this.getEnd(d).getTime()))

      if (startMin && endMax) {
        this.updateAxisX(new Date(startMin), new Date(endMax), animDuration)
      }
    },
    updateAxisX (start, end, animDuration = 200) {
      const trans = d3
        .transition()
        .ease(d3.easeLinear)
        .duration(animDuration)

      this.scaleX.domain([start, end])
      this.elAxisX.transition(trans).call(this.axisX)

      this.elContainer
        .selectAll('.slot')
        .transition(trans)
        .attr(
          'transform',
          d =>
            `translate (${this.scaleX(this.getStart(d))}, ${this.scaleY(
              d.name
            )})`
        )

      this.elContainer
        .selectAll('.slot')
        .transition(trans)
        .select('.zone')
        .attr(
          'width',
          d => this.scaleX(this.getEnd(d)) - this.scaleX(this.getStart(d))
        )

      this.elContainer
        .selectAll('.slot')
        .transition(trans)
        .select('.handlerRight')
        .attr(
          'x',
          d => this.scaleX(this.getEnd(d)) - this.scaleX(this.getStart(d)) - 5
        )

      this.elMain
        .select('.dateStartAll')
        .transition(trans)
        .attr('x1', d => this.scaleX(this.dateStartAll))
        .attr('x2', d => this.scaleX(this.dateStartAll))
    },
    updateAxisY () {
      const trans = d3
        .transition()
        .ease(d3.easeLinear)
        .duration(200)

      this.scaleY.domain(this.getUniqList())
      this.elAxisY.transition(trans).call(this.axisY)

      this.elContainer
        .selectAll('.slot')
        .transition(trans)
        .attr(
          'transform',
          d =>
            `translate (${this.scaleX(this.getStart(d))}, ${this.scaleY(
              d.name
            )})`
        )

      const slotHeight = this.getSlotHeight()

      this.elContainer
        .selectAll('.slot')
        .select('.zone')
        .transition(trans)
        .attr('y', -slotHeight / 2)
        .attr('height', slotHeight)

      this.elContainer
        .selectAll('.slot')
        .select('.handlerLeft')
        .transition(trans)
        .attr('y', d => -slotHeight / 2)
        .attr('height', slotHeight)

      this.elContainer
        .selectAll('.slot')
        .select('.handlerRight')
        .transition(trans)
        .attr('y', d => -slotHeight / 2)
        .attr('height', slotHeight)
    },
    moveStartDate (duration) {
      updateStartDate(new Date(this.dateStartAll.getTime() + duration * 1000))
    },
    updateStartDate (date) {
      date.setSeconds(0)
      date.setMilliseconds(0)

      const diffDuration = getDurationBetween(this.dateStartAll, date)

      this.dateStartAll = date
      document.getElementById('startDate').innerHTML = formatDate(
        this.dateStartAll
      )

      this.elMain
        .select('.dateStartAll')
        .attr('x1', this.scaleX(this.dateStartAll))
        .attr('x2', this.scaleX(this.dateStartAll))

      this.elContainer
        .selectAll('.slot')
        .attr(
          'transform',
          d =>
            `translate (${this.scaleX(this.getStart(d))}, ${this.scaleY(
              d.name
            )})`
        )

      this.updateData()

      const [start, end] = this.scaleX.domain()
      this.updateAxisX(
        getDatePlusDuration(start, diffDuration),
        getDatePlusDuration(end, diffDuration),
        0
      )

      if (this.currentSlot) {
        this.selectSlot(this.currentSlot.id)
      }
    },
    updateData () {
      const self = this
      const updateSlots = this.elContainer
        .selectAll('.slot')
        .data(self.data, d => d.id)
      const exitSlots = updateSlots.exit()
      const enterSlots = updateSlots.enter()

      exitSlots.remove()

      exitSlots
        .transition()
        .duration(500)
        .attr('opacity', 0)

      const slotHeight = self.getSlotHeight()

      const entry = enterSlots
        .append('g')
        .attr('class', 'slot')
        .attr('uniqid', d => 'id' + d.id)
        .attr('name', d => d.name)
        .attr(
          'transform',
          d =>
            `translate (${self.scaleX(self.getStart(d))}, ${self.scaleY(
              d.name
            )})`
        )

      entry
        .append('rect')
        .attr('class', 'zone')
        .attr('x', 0)
        .attr('y', -slotHeight / 2)
        .attr(
          'width',
          d => self.scaleX(self.getEnd(d)) - self.scaleX(self.getStart(d))
        )
        .attr('height', slotHeight)
        .attr('cursor', 'move')
        .style('fill', d => 'rgba (156,200,176,1)')
        .on('dblclick', zoneDblClick)
        .call(
          d3
            .drag()
            .on('start', dragZoneStart)
            .on('drag', dragZoneProgress)
            .on('end', dragZoneEnd)
        )

      entry
        .append('rect')
        .attr('class', 'handlerLeft')
        .attr('x', d => -5)
        .attr('y', d => -slotHeight / 2)
        .attr('width', 10)
        .attr('height', slotHeight)
        .attr('cursor', 'ew-resize')
        .style('fill', 'grey')
        .attr('fill-opacity', 0)
        .call(
          d3
            .drag()
            .on('start', dragLeftStart)
            .on('drag', dragLeftProgress)
            .on('end', dragLeftEnd)
        )

      entry
        .append('rect')
        .attr('class', 'handlerRight')
        .attr(
          'x',
          d => self.scaleX(self.getEnd(d)) - self.scaleX(self.getStart(d)) - 5
        )
        .attr('y', d => -slotHeight / 2)
        .attr('width', 10)
        .attr('height', slotHeight)
        .attr('cursor', 'ew-resize')
        .style('fill', 'grey')
        .attr('fill-opacity', 0)
        .call(
          d3
            .drag()
            .on('start', dragRightStart)
            .on('drag', dragRightProgress)
            .on('end', dragRightEnd)
        )

      let startDragMouseX = null
      let startDragDuration = null
      let startDragDate = null

      function zoneDblClick (slot) {
        self.removeSlot(slot.id)
      }

      function dragZoneStart (slot) {
        startDragMouseX = d3.mouse(this)[0]

        self.selectSlot(slot.id)
      }

      // UPDATE START
      function dragZoneProgress (slot) {
        const moveX = d3.event.x - startDragMouseX

        const currSlotPosPixelX = self.scaleX(self.getStart(slot))
        const tempStartDate = self.scaleX.invert(currSlotPosPixelX + moveX)
        tempStartDate.setSeconds(0)
        tempStartDate.setMilliseconds(0)

        const newValue = self.calculateNewValue(
          slot,
          tempStartDate,
          slot.duration,
          'move'
        )
        slot.startAfter = newValue.startAfter
        slot.duration = newValue.duration

        // change g.slot translation
        d3.select(this.parentNode).attr(
          'transform',
          `translate (${self.scaleX(self.getStart(slot))}, ${self.scaleY(
            slot.name
          )})`
        )

        self.selectSlot(slot.id)
      }

      function dragZoneEnd (slot) {
        startDragMouseX = null
      }

      function dragLeftStart (slot) {
        if (d3.event.sourceEvent.screenX) {
          startDragMouseX = d3.event.sourceEvent.screenX
        } else if (
          d3.event.sourceEvent.touches &&
          d3.event.sourceEvent.touches[0]
        ) {
          startDragMouseX = d3.event.sourceEvent.touches[0].screenX
        }

        startDragDuration = slot.duration
        startDragDate = this.getStart(slot)

        this.selectSlot(slot.id)
      }
      // UPDATE START & DURATION
      function dragLeftProgress (slot) {
        let moveX = d3.event.sourceEvent.screenX - startDragMouseX
        if (d3.event.sourceEvent.screenX) {
          moveX = d3.event.sourceEvent.screenX - startDragMouseX
        } else if (
          d3.event.sourceEvent.touches &&
          d3.event.sourceEvent.touches[0]
        ) {
          moveX = d3.event.sourceEvent.touches[0].screenX - startDragMouseX
        }

        const currPosX = self.scaleX(startDragDate)
        const newPosX = self.scaleX(startDragDate) + moveX

        const tempStartDate = self.scaleX.invert(newPosX)
        tempStartDate.setSeconds(0)
        tempStartDate.setMilliseconds(0)

        const endDate = getDatePlusDuration(startDragDate, startDragDuration)
        let tempDuration = (endDate.getTime() - tempStartDate.getTime()) / 1000
        tempDuration = Math.round(tempDuration / self.GAP) * self.GAP

        const newValue = self.calculateNewValue(
          slot,
          tempStartDate,
          tempDuration,
          'left'
        )
        slot.startAfter = newValue.startAfter
        slot.duration = newValue.duration

        // change g.slot translation
        d3.select(this.parentNode).attr(
          'transform',
          `translate (${self.scaleX(self.getStart(slot))}, ${self.scaleY(
            slot.name
          )})`
        )

        // change rect.zone width
        d3.select(this.parentNode)
          .select('.zone')
          .attr(
            'width',
            self.scaleX(self.getEnd(slot)) - self.scaleX(self.getStart(slot))
          )

        // change rect.handlerRight x
        d3.select(this.parentNode)
          .select('.handlerRight')
          .attr(
            'x',
            self.scaleX(self.getEnd(slot)) -
              self.scaleX(self.getStart(slot)) -
              5
          )

        self.selectSlot(slot.id)
      }
      function dragLeftEnd (slot) {
        startDragMouseX = null
        startDragDuration = null
        startDragDate = null
      }

      function dragRightStart (slot) {
        startDragMouseX = d3.mouse(this)[0]
        startDragDuration = slot.duration

        self.selectSlot(slot.id)
      }
      // UPDATE DURATION
      function dragRightProgress (slot) {
        const moveX = d3.event.x - startDragMouseX
        const currSlotWidthPixelX = self.scaleX(
          getDatePlusDuration(self.getStart(slot), startDragDuration)
        )

        const tempEndDate = self.scaleX.invert(currSlotWidthPixelX + moveX)
        let tempDuration =
          (tempEndDate.getTime() - self.getStart(slot).getTime()) / 1000
        tempDuration = Math.round(tempDuration / self.GAP) * self.GAP

        const newValue = self.calculateNewValue(
          slot,
          self.getStart(slot),
          tempDuration,
          'right'
        )
        slot.startAfter = newValue.startAfter
        slot.duration = newValue.duration

        // change rect.zone width
        d3.select(this.parentNode)
          .select('.zone')
          .attr(
            'width',
            self.scaleX(self.getEnd(slot)) - self.scaleX(self.getStart(slot))
          )

        // change rect.handlerRight x
        d3.select(this.parentNode)
          .select('.handlerRight')
          .attr(
            'x',
            self.scaleX(self.getEnd(slot)) -
              self.scaleX(self.getStart(slot)) -
              5
          )

        self.selectSlot(slot.id)
      }
      function dragRightEnd (slot) {
        startDragMouseX = null
        startDragDuration = null
      }
    }, // end updateData()
    getEnd (slot) {
      return getDatePlusDuration(this.getStart(slot), slot.duration)
    },
    getStart (slot) {
      return getDatePlusDuration(this.dateStartAll, slot.startAfter)
    },
    buildDom () {
      const self = this
      this.scaleX = d3
        .scaleTime()
        .range([0, this.WIDTH - this.BORDER * 2])
        .domain([
          this.dateStartAll,
          getDatePlusDuration(this.dateStartAll, 15 * 60)
        ])

      this.scaleY = d3
        .scalePoint()
        .range([this.HEIGHT - this.BORDER * 2, 0])
        .padding(0.5)
        .domain(this.getUniqList())

      this.axisX = d3
        .axisBottom(this.scaleX)
        .tickSizeInner(-this.HEIGHT + this.BORDER * 2)
      this.axisY = d3
        .axisLeft(this.scaleY)
        .tickSizeInner(-this.WIDTH + this.BORDER * 2)

      this.svg = d3
        .select('#chart')
        .append('svg')
        .attr('width', this.WIDTH)
        .attr('height', this.HEIGHT)

      this.svg
        .append('defs')
        .append('clipPath')
        .attr('id', 'clip')
        .append('rect')
        .attr('width', this.WIDTH - this.BORDER * 2)
        .attr('height', this.HEIGHT - this.BORDER * 2)
        .style('fill', 'red')

      this.elMain = this.svg
        .append('g')
        .attr('class', 'main')
        .attr(
          'transform',
          'translate (' + this.BORDER + ',' + this.BORDER + ')'
        )
        .call(d3.zoom().on('zoom', scrollZoom))
        .on('dblclick.zoom', null)

      this.elAxisX = this.elMain
        .append('g')
        .attr('class', 'axis axis--x')
        .attr(
          'transform',
          'translate (0,' + (this.HEIGHT - this.BORDER * 2) + ')'
        )
        .call(this.axisX)

      this.elAxisY = this.elMain
        .append('g')
        .attr('class', 'axis axis--y')
        .call(this.axisY)

      let elDragZone = this.elMain
        .append('rect')
        .attr('class', 'drag-zone')
        .attr('width', this.WIDTH - this.BORDER * 2)
        .attr('height', this.HEIGHT - this.BORDER * 2)
        .attr('clip-path', 'url (#clip)')
        .call(d3.drag().on('drag', dragProgress))

      this.elContainer = this.elMain
        .append('g')
        .attr('class', 'container')
        .attr('clip-path', 'url (#clip)')

      let elDateStartAll = this.elMain
        .append('line')
        .attr('class', 'dateStartAll')
        .attr('clip-path', 'url (#clip)')
        .attr('x1', this.scaleX(this.dateStartAll))
        .attr('y1', this.scaleY.range()[0])
        .attr('x2', this.scaleX(this.dateStartAll))
        .attr('y2', this.scaleY.range()[1])

      let startZoomK = 1

      function dragProgress () {
        const [start, end] = self.scaleX.domain()
        const newStart = self.scaleX.invert(-d3.event.dx)
        const duration = getDurationBetween(start, newStart)

        self.updateAxisX(
          getDatePlusDuration(start, duration),
          getDatePlusDuration(end, duration),
          0
        )
      }
      function scrollZoom () {
        if (d3.event.transform.k > startZoomK) {
          self.zoomAxisX('in')
        } else if (d3.event.transform.k < startZoomK) {
          self.zoomAxisX('out')
        }
        startZoomK = d3.event.transform.k
      }
    }, // end buildDom()
    calculateNewValue (slot, tempStart, tempDuration, action) {
      const newValue = {
        startAfter: getDurationBetween(this.dateStartAll, tempStart),
        duration: tempDuration
      }

      // Make sure GAP as minimum
      if (newValue.duration < this.GAP) {
        newValue.duration = this.GAP
      }

      let startMin = 0
      let endMax = null

      // Check if intersection
      const commonSlots = this.data.filter(
        d => d.name === slot.name && d.id !== slot.id
      )
      if (commonSlots.length > 0) {
        const orderedCommonSlots = commonSlots
          .map(s => ({
            start: s.startAfter,
            end: s.startAfter + s.duration,
            duration: s.duration,
            id: s.id
          }))
          .sort((a, b) => (a.start < b.start ? -1 : 1))

        // DETERMINE IF SLOT IS:
        const tabSlotFirst = orderedCommonSlots[0]
        const tabSlotLast = orderedCommonSlots[orderedCommonSlots.length - 1]

        // BEFORE ALL SLOTS > endMax = startFirst - GAP
        if (slot.startAfter + slot.duration <= tabSlotFirst.start - this.GAP) {
          endMax = tabSlotFirst.start - this.GAP
          // console.log ('BEFORE ALL > endMax = ', endMax)
        } else if (
          slot.startAfter >=
          tabSlotLast.start + tabSlotLast.duration + this.GAP
        ) {
          startMin = tabSlotLast.start + tabSlotLast.duration + this.GAP
          // console.log ('AFTER ALL > startMin = ', startMin)
        } else {
          for (let i = 0; i < orderedCommonSlots.length; i++) {
            const tabSlotA = orderedCommonSlots[i]
            const tabSlotB = orderedCommonSlots[i + 1]

            if (
              slot.startAfter >=
              tabSlotA.start + tabSlotA.duration + this.GAP
            ) {
              startMin = tabSlotA.start + tabSlotA.duration + this.GAP

              if (
                tabSlotB &&
                slot.startAfter + slot.duration <= tabSlotB.start - this.GAP
              ) {
                endMax = tabSlotB.start - this.GAP
                break
              }
            }
          }
          // console.log ('BETWEEN 2 SLOTS > startMin = ', startMin, ' > endMax = ', endMax)
        }
      }

      if (action === 'move') {
        if (newValue.startAfter < startMin) {
          newValue.startAfter = startMin
        }
        if (endMax && newValue.startAfter + newValue.duration > endMax) {
          newValue.startAfter = endMax - newValue.duration
        }
      } else if (action === 'left') {
        if (newValue.startAfter < startMin) {
          newValue.startAfter = startMin

          // Be sure to keep current end
          const currEnd = slot.startAfter + slot.duration
          newValue.duration = currEnd - newValue.startAfter
        } else if (newValue.startAfter >= slot.startAfter + slot.duration) {
          newValue.startAfter = slot.startAfter + slot.duration - this.GAP
        }
      } else if (action === 'right') {
        if (endMax && newValue.startAfter + newValue.duration > endMax) {
          newValue.duration = endMax - newValue.startAfter
        }
      }

      return newValue
    },
    selectSlot (id) {
      const slot = this.data.find(s => s.id === id)

      // Unselect all slot
      this.elContainer
        .selectAll('.slot')
        .select('.zone')
        .classed('active', false)

      if (slot) {
        this.currentSlot = slot

        this.elContainer
          .selectAll(`.slot[uniqid='id${slot.id}']`)
          .select('.zone')
          .classed('active', true)

        document.getElementById('selection').innerHTML = `
                    <div style='background: grey'>
                        <h3>NAME: ${this.currentSlot.name}  (id=${
  this.currentSlot.id
})<br>
                        START: ${formatDate(
    this.getStart(this.currentSlot)
  )}<br>
                        END: ${formatDate(this.getEnd(this.currentSlot))}
                        <button onClick='removeSlot (${
  this.currentSlot.id
})'>REMOVE</button>
                        </h3>
                    </div>`
      } else {
        this.currentSlot = null

        document.getElementById('selection').innerHTML = ``
      }
    },
    removeSlot (id) {
      const idx = this.data.findIndex(s => s.id === id)
      this.data.splice(idx, 1)
      this.updateData()
      this.updateAxisY()

      if (this.currentSlot && this.currentSlot.id === id) {
        this.selectSlot(-1)
      }
    },
    addSlot (name) {
      const newId = (_.max(_.map(this.data, 'id')) || 1) + 1
      const newData = { id: newId, name, startAfter: 0, duration: 1 * 60 }

      const commonSlots = this.data.filter(s => s.name === name)
      if (commonSlots.length > 0) {
        const maxSlotEnd = _.max(
          _.map(commonSlots, d => d.startAfter + d.duration)
        )
        newData.startAfter = maxSlotEnd + this.GAP
      }

      this.data.push(newData)
      this.updateData()

      // update selected slot
      this.selectSlot(newData.id)

      // make sure new one is visible
      const [start, end] = this.scaleX.domain()
      const newOneStart = this.getStart(newData)
      const newOneEnd = this.getEnd(newData)

      if (
        isDateBeforeOther(newOneStart, start) ||
        isDateAfterOther(newOneEnd, end)
      ) {
        this.zoomViewAllAxisX(0)
      }
      this.updateAxisY()
    },
    removeAllSlot () {
      this.data = []

      this.updateData()
      this.selectSlot(-1)
      this.updateAxisY()
    },
    buildAutoSlot () {
      const list = [
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'X'
      ]

      const tempData = []
      list.forEach((name, i) => {
        tempData.push({
          id: i + 1,
          name,
          startAfter: i * this.GAP * 2,
          duration: this.GAP
        })
      })

      this.data = tempData

      this.updateData()
      this.selectSlot(-1)
      this.zoomViewAllAxisX(0)
      this.updateAxisY()
    },
    getUniqList () {
      return _.uniq(_.map(this.data, 'name'))
        .sort()
        .reverse()
    },
    getSlotHeight () {
      const l = this.getUniqList().length

      return Math.ceil((this.HEIGHT - 2 * this.BORDER) / l) - (l > 10 ? 0 : 4)
    }
  },
  mounted () {
    this.data.push(...this.data2)
    this.WIDTH = this.$refs.chartdiv.clientWidth
    this.dateStartAll.setMinutes(this.dateStartAll.getMinutes() + 10)
    this.dateStartAll.setSeconds(0)
    this.dateStartAll.setMilliseconds(0)

    this.buildDom()
    this.updateData()
  },
  filters: {
    formatDate (d) {
      return formatDate(d)
    }
  }
}
function getDatePlusDuration (date, duration) {
  return new Date(date.getTime() + duration * 1000)
}
function getDurationBetween (dateA, dateB) {
  return (dateB.getTime() - dateA.getTime()) / 1000
}
function formatDate (d) {
  let h = d.getHours()
  let m = d.getMinutes()
  let s = d.getSeconds()

  if (h < 10) h = '0' + h
  if (m < 10) m = '0' + m
  if (s < 10) s = '0' + s

  return `${h}:${m}:${s}+${d.getMilliseconds()}`
}
function isDateAfterOther (dateA, dateB, gap = 0) {
  return dateA.getTime() + gap * 1000 > dateB.getTime()
}

function isDateBeforeOther (dateA, dateB, gap = 0) {
  return dateA.getTime() + gap * 1000 < dateB.getTime()
}
</script>

<style lang="stylus">
.ressource-scheduler {
  .tick line {
    opacity: 0.2
  }

  .dateStartAll {
    stroke: green
  }

  .zone.active {
    stroke: blue
    stroke-width: 3px
  }

  .drag-zone {
    opacity: 0
  }

  #chart {
    min-width: 250px
  }
}
</style>
