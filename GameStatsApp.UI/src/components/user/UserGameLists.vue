<template>
    <div>
        <div class="show-md d-flex flex-column flex-shrink-0 p-3 position-absolute top-0 start-0 bg-light" style="width: 280px; height: 100vh; margin-top: 63px;">
            <ul class="nav nav-pills flex-column mb-auto">
                <li v-for="(item, itemIndex) in items.filter(i => i.isDefault)" key="item.id" class="nav-item">
                    <a @click="gamelistid = item.id" href="#" class="nav-link" :class="{ 'active' : gamelistid == item.id }">
                        <font-awesome-icon :icon="getIconClass(item.name)" size="lg" class="me-3"/>
                        <span>{{ item.name }}</span>
                    </a>
                </li>
                <li v-if="items.filter(i => !i.isDefault).length > 0" class="border-top my-3"></li>
                <li v-for="(item, itemIndex) in items.filter(i => !i.isDefault)" key="item.id" class="nav-item">
                    <a @click="gamelistid = item.id" href="#" class="nav-link" :class="{ 'active' : gamelistid == item.id }">
                        <span>{{ item.name }}</span>
                    </a>
                </li>                
            </ul>
        </div>
        <div class="show-sm">
            <div class="row g-2 justify-content-center">
                <div class="btn-group">
                    <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                        <font-awesome-icon v-if="getIconClass(items.find(i => i.id == gamelistid)?.name)" :icon="getIconClass(items.find(i => i.id == gamelistid)?.name)" size="lg"/>
                        <span class="mx-auto">{{ items.find(i => i.id == gamelistid)?.name }}</span>
                    </button>
                    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                        <li v-for="(item, itemIndex) in items">
                            <a :key="item.id" @click="gamelistid = item.id" class="dropdown-item" :class="{ 'active' : gamelistid == item.id }" href="#/" data-toggle="pill">{{ item.name }}</a>
                        </li>
                    </ul>
                </div>                
            </div>
        </div>
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
                    case "All Games":
                        iconClass = 'fa-solid fa-list';
                        break;
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






