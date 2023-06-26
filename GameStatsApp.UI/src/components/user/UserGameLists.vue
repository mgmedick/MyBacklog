<template>
    <div id="divTabContainer">
        <nav class="nav nav-pills flex-column flex-sm-row">
            <a @click="gamelistid = 0" href="#" class="flex-sm-fill text-center nav-link" :class="{ 'active' : gamelistid == 0 }">
                <font-awesome-icon icon="fa-solid fa-list" size="lg"/>
                All Games
            </a>
            <a v-for="(item, itemIndex) in items.filter(i => i.isDefault)" @click="gamelistid = item.id" href="#" class="flex-sm-fill text-center nav-link" :class="{ 'active' : gamelistid == item.id }">
                <font-awesome-icon :icon="getIconClass(item.name)" size="lg"/>
                {{ item.name }}
            </a>
            <button-dropdown v-if="items.filter(i => !i.isDefault).length > 1" class="more py-1 pr-1" :btnclasses="'btn-secondary'">
                <template v-slot:text>
                    <span>...</span>
                </template>
                <template v-slot:options>
                    <template v-for="(item, itemIndex) in items.filter(i => !i.isDefault)" :key="item.id">
                        <a @click="gamelistid = item.id" class="dropdown-item d-none" :class="{ 'active' : gamelistid == item.id }" href="#/" data-toggle="pill">{{ item.name }}</a>
                    </template>
                </template>
            </button-dropdown>              
        </nav>        
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






