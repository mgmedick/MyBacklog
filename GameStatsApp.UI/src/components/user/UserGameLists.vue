<template>
    <div>
        <div class="show-md d-flex flex-column flex-shrink-0 p-3 position-absolute top-0 start-0 bg-light" style="width: 280px; height: 100vh; margin-top: 63px;">
            <ul class="nav nav-pills flex-column mb-auto">
                <li class="nav-item">
                    <a @click="gamelistid = 0" href="#" class="nav-link" :class="{ 'active' : gamelistid == 0 }">
                        <font-awesome-icon icon="fa-solid fa-list" size="lg" class="me-3"/>
                        <span>All Games</span>
                    </a>
                </li>
                <li v-for="(item, itemIndex) in items.filter(i => i.isDefault)" key="item.id" class="nav-item">
                    <a @click="gamelistid = item.id" href="#" class="nav-link" :class="{ 'active' : gamelistid == item.id }">
                        <font-awesome-icon :icon="getIconClass(item.name)" size="lg" class="me-3"/>
                        <span>{{ item.name }}</span>
                    </a>
                </li>
                <li v-if="items.filter(i => !i.isDefault).length > 0" class="border-top my-3"></li>
                <li v-for="(item, itemIndex) in items.filter(i => !i.isDefault)" key="item.id" class="nav-item">
                    <a @click="gamelistid = item.id" href="#" class="nav-link" :class="{ 'active' : gamelistid == item.id }">
                        <font-awesome-icon :icon="getIconClass(item.name)" size="lg" class="me-2"/>
                        <span>{{ item.name }}</span>
                    </a>
                </li>                
            </ul>
        </div>
        <div class="show-sm">
            <div class="row g-2 justify-content-center">
                <button-dropdown v-if="items.filter(i => !i.isDefault).length > 0" :btnclasses="'btn-outline-primary'">
                    <template v-slot:text>
                        <span class="mx-auto"><font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/></span>
                    </template>
                    <template v-slot:options>
                        <template v-for="(item, itemIndex) in items.filter(i => !i.isDefault)" :key="item.id">
                            <a @click="gamelistid = item.id" class="dropdown-item d-none" :class="{ 'active' : gamelistid == item.id }" href="#/" data-toggle="pill">{{ item.name }}</a>
                        </template>
                    </template>
                </button-dropdown>                
            </div>
        </div>

        <!-- <nav class="nav nav-pills flex-column flex-sm-row">
            <a @click="gamelistid = 0" href="#" class="d-flex flex-sm-fill nav-link" :class="{ 'active' : gamelistid == 0 }">
                <font-awesome-icon icon="fa-solid fa-list" size="lg"/>
                <span class="mx-auto">All Games</span>
            </a>
            <a v-for="(item, itemIndex) in items.filter(i => i.isDefault)" @click="gamelistid = item.id" href="#" class="d-flex flex-sm-fill nav-link nav-link-outline-primary" :class="{ 'active' : gamelistid == item.id }">
                <font-awesome-icon :icon="getIconClass(item.name)" size="lg"/>
                <span class="mx-auto">{{ item.name }}</span>
            </a>
            <button-dropdown v-if="items.filter(i => !i.isDefault).length > 0" class="more py-1 pr-1" :btnclasses="'btn-outline-primary'">
                <template v-slot:text>
                    <font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/>
                </template>
                <template v-slot:options>
                    <template v-for="(item, itemIndex) in items.filter(i => !i.isDefault)" :key="item.id">
                        <a @click="gamelistid = item.id" class="dropdown-item d-none" :class="{ 'active' : gamelistid == item.id }" href="#/" data-toggle="pill">{{ item.name }}</a>
                    </template>
                </template>
            </button-dropdown>              
        </nav>        
        <div class="btn-group" role="group" aria-label="Basic radio toggle button group">
            <input type="radio" class="btn-check" name="btnradio" id="btnradio1" autocomplete="off" checked>
            <label class="btn btn-outline-primary" for="btnradio1">Radio 1</label>

            <input type="radio" class="btn-check" name="btnradio" id="btnradio2" autocomplete="off">
            <label class="btn btn-outline-primary" for="btnradio2">Radio 2</label>

            <input type="radio" class="btn-check" name="btnradio" id="btnradio3" autocomplete="off">
            <label class="btn btn-outline-primary" for="btnradio3">Radio 3</label>
        </div> -->
    </div>    
</template>
<script>
    import axios from 'axios';

    export default {
        name: "UserGameList",
        props: {
            userid: String
        },
        data: function () {
            return {
                items: [],
                gamelistid: 0
            };
        },
        watch: {},
        created: function () {
            this.loadData();
        },
        mounted: function() {
            window.addEventListener('resize', this.resizeTabs);
        },
        methods: {
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetUserGameLists', { params: { userID: this.userid } })
                    .then(res => {
                        that.items = res.data;               
                        // if (!that.gamelistid) {
                        //     that.gamelistid = res.data[0]?.id;
                        // }
                        that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            getIconClass: function (name) {
                var iconClass = '';

                switch (name) {
                    case "Backlog":
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case "Playing":
                        iconClass = 'fa-solid fa-play';
                        break;
                    case "Complete":
                        iconClass = 'fa-solid fa-check';
                        break;
                }

                return iconClass;
            },            
        },
    };
</script>






