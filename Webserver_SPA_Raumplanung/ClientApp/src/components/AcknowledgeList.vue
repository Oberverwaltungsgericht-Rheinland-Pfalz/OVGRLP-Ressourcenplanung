<template>
  <table>
    <thead>
        <tr>
            <th>Bearbeiten</th>
            <th>Name</th>
            <th>Status</th>
            <th>Raum</th>
            <th>Datum</th>
        </tr>
    </thead>
    <tbody class="appointment-open-list">
        <tr v-for="(item, idx) in list" v-bind:key="idx">
            <td>
                <v-menu bottom offset-y>
                    <template v-slot:activator="{ on }">
                        <v-btn v-on="on">Bearbeiten</v-btn>
                    </template>
                    <v-list>
                        <v-list-tile @click="acknowledge(item)">
                            <v-list-tile-title>
                                <v-icon v-html="'done'"></v-icon> Bestätigen
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="reject(item)">
                            <v-list-tile-title>
                                <v-icon v-html="'close'"></v-icon> Ablehnen
                            </v-list-tile-title>
                        </v-list-tile>
                        <v-list-tile @click="move(item)">
                            <v-list-tile-title>
                                <v-icon v-html="'create'"></v-icon>  Verschieben
                            </v-list-tile-title>
                        </v-list-tile>
                    </v-list>
                </v-menu>
            </td>
            <td>{{item.title}}</td>
            <td>{{item.status}}</td>
            <td>{{item.ressource}}</td>
            <td>{{item.dateTime}}</td>
        </tr>
    </tbody>
  </table>
</template>

<script>
import {VMenu} from 'vuetify'
export default {
    components: { VMenu},
    data() {
        return {
            list: [
                {title: 'Meeting', status: 'offen', ressource: 'Raum1', dateTime: new Date()},
                {title: 'Meeting2', status: 'offen', ressource: 'Raum2', dateTime: new Date()}
            ]
        }
    },
    methods: {
        acknowledge() {
            // change appointment status to accepted
            return true
        },
        reject() {
            // change appointment status to rejected
            return false
        },
        move() {
            // change appointment status to changed and change the date
            return false
        }
    }
}
</script>

<style scoped lang="stylus">
.appointment-open-list:nth-of-type(2n)
  background-color lightgrey

</style>

