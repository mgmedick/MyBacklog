<template>
    <div class="container games-container">
        <div class="row">
            <div class="col-lg-2 col-md-3 col-4 mb-3">
                <div class="bg-light d-flex" style="height: 100%;">
                    <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px; padding-top: 50%; padding-bottom: 50%;"/>
                </div>
            </div>
            <div v-for="(game, gameIndex) in games" class="col-lg-2 col-md-3 col-4 mb-3 game-image-container" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                <img :src="game.coverImagePath" class="img-fluid rounded" alt="Responsive image">
                <div class="game-list-icons" :class="{ 'd-none' : usergamelists.filter(i => i.defaultGameListID != 1 && game.userGameListIDs.indexOf(i.id) > -1).length == 0 }" style="margin-top: -60px;">
                    <div class="btn-group btn-group-sm p-3" role="group" style="width: 100%;">
                        <button v-for="(userGameList, userGameListIndex) in usergamelists.filter(i => i.defaultGameListID && i.defaultGameListID != 1)" @click="onGameListIconClick($event, userGameList, game)" type="button" class="btn btn-light" :class="{ 'active' : game.userGameListIDs.indexOf(userGameList.id) > -1 }">
                            <font-awesome-icon :icon="getIconClass(userGameList.defaultGameListID)" size="lg"/>
                        </button>
                        <div v-if="usergamelists.filter(i => !i.defaultGameListID).length > 0" class="btn-group btn-group-sm" role="group">
                            <button type="button" class="btn btn-light dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/>
                            </button>
                            <ul class="dropdown-menu">
                                <li v-for="(userGameList, userGameListIndex) in usergamelists.filter(i => !i.defaultGameListID)" ><a class="dropdown-item" href="#" :class="{ 'active' : game.userGameListIDs.indexOf(userGameList.id) > -1 }">{{ userGameList.name }}</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios';
    import { Toast } from 'bootstrap';

    export default {
        name: "UserGameListGames",
        props: {
            userid: String,
            usergamelistid: Number,
            usergamelists: Array
        },
        data: function () {
            return {
                games: []
            };
        },       
        watch: {
            usergamelistid: function (val, oldVal) {
                this.loadData();
            }           
        },  
        created: function () {
            this.loadData();
        },
        mounted: function() {
            var that = this;
        },
        methods: {
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetUserGameListGames', { params: { userGameListID: this.usergamelistid } })
                    .then(res => {
                        that.games = res.data;                        
                        that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            getIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-list';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 3:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 4:
                        iconClass = 'fa-solid fa-check';
                        break;
                }

                return iconClass;
            },      
            onGameImageMouseOver(e) {
                e.target.closest('.game-image-container').querySelector('.game-list-icons').classList.remove('d-none');
            },     
            onGameImageMouseLeave(e) {
                var container = e.target.closest('.game-image-container');
                if (!container.classList.contains('active') && !container.querySelector('.game-list-icons button.active')) {
                    container.querySelector('.game-list-icons').classList.add('d-none');
                }
            }, 
            onGameImageClick(e) {
                var container = e.target.closest('.game-image-container');
                if (!container.classList.contains('active')) {
                    container.querySelector('.game-list-icons').classList.remove('d-none');
                    container.classList.add('active');
                } else {
                    container.querySelector('.game-list-icons').classList.add('d-none');
                    container.classList.remove('active');                    
                }
            },  
            onGameListIconClick(e, userGameList, game) {
                var el = e.target.closest('button');
                if (!el.classList.contains('active')) {
                    this.addGameToUserGameList(el, userGameList, game)
                } else {
                    this.removeGameFromUserGameList(el, userGameList, game)
                }
            },
            addGameToUserGameList(el, userGameList, game) {
                var that = this;

                return axios.post('/User/AddGameToUserGameList', null,{ params: { userGameListID: userGameList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            el.classList.add('active');                           
                            that.$emit('success', "Successfully added " + game.name + " to " + userGameList.name);
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },          
            removeGameFromUserGameList(el, userGameList, game) {
                var that = this;
                
                return axios.post('/User/RemoveGameFromUserGameList', null,{ params: { userGameListID: userGameList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            el.classList.remove('active');                           
                            that.$emit('success', "Successfully removed " + game.name + " from " + userGameList.name);
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }                
        },
    };
</script>






