<template>
    <div>
        <div class="toast-container position-absolute sticky-top p-3 top-0 end-0" id="toastPlacement" style="margin-top: 70px;"> 
            <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span>{{ errorMessage }}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>           
            <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span>{{ successMessage }}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        </div>
        <div class="show-md d-flex flex-column flex-shrink-0 p-3 position-absolute top-0 start-0 bg-light" style="width: 280px; height: 100vh; margin-top: 63px;">
            <ul class="nav nav-pills flex-column mb-auto">
                <li v-for="(item, itemIndex) in items.filter(i => i.defaultGameListID)" key="item.id" class="nav-item">
                    <a @click="selectedItem = item" href="#" class="nav-link" :class="{ 'active' : selectedItem.id == item.id }">
                        <font-awesome-icon :icon="getIconClass(item.defaultGameListID)" size="lg" class="me-3"/>
                        <span>{{ item.name }}</span>
                    </a>
                </li>
                <li v-if="items.filter(i => !i.defaultGameListID).length > 0" class="border-top my-3"></li>
                <li v-for="(item, itemIndex) in items.filter(i => !i.defaultGameListID)" key="item.id" class="nav-item">
                    <a @click="selectedItem = item" href="#" class="nav-link" :class="{ 'active' : selectedItem.id == item.id }">
                        <span>{{ item.name }}</span>
                    </a>
                </li>                
            </ul>
        </div>
        <div class="show-sm row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="selectedItem && selectedItem.defaultGameListID" :icon="getIconClass(selectedItem.defaultGameListID)" size="lg"/>
                    <span class="mx-auto">{{ items.find(i => i.id == selectedItem.id)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li v-for="(item, itemIndex) in items">
                        <a :key="item.id" @click="selectedItem = item" class="dropdown-item" :class="{ 'active' : selectedItem.id == item.id }" href="#/" data-toggle="pill">{{ item.name }}</a>
                    </li>
                </ul>
            </div>                
        </div>
        <div class="container games-container">
            <div v-if="selectedItem" class="row">
                <div class="col-lg-2 col-md-3 col-4 mb-3">
                    <div class="bg-light d-flex" style="height: 100%;">
                        <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px; padding-top: 50%; padding-bottom: 50%;"/>
                    </div>
                </div>
                <div v-for="(game, gameIndex) in selectedItem.gameVMs" class="col-lg-2 col-md-3 col-4 mb-3 game-image-container" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                    <img :src="game.coverImagePath" class="img-fluid rounded" alt="Responsive image">
                    <div class="game-list-icons" :class="{ 'd-none' : items.filter(i => i.defaultGameListID != 1 && i.gameVMs.filter(h => h.id == game.id).length > 0).length == 0 }" style="margin-top: -60px;">
                        <div class="btn-group btn-group-sm p-3" role="group" style="width: 100%;">
                            <button v-for="(item, itemIndex) in items.filter(i => i.defaultGameListID && i.defaultGameListID != 1)" @click="onGameListIconClick($event, item, game)" type="button" class="btn btn-light" :class="{ 'active' : item.gameVMs.filter(h => h.id == game.id).length > 0 }">
                                <font-awesome-icon :icon="getIconClass(item.defaultGameListID)" size="lg"/>
                            </button>
                            <div v-if="items.filter(i => !i.defaultGameListID).length > 0" class="btn-group btn-group-sm" role="group">
                                <button type="button" class="btn btn-light dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                    <font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/>
                                </button>
                                <ul class="dropdown-menu">
                                    <li v-for="(item, itemIndex) in items.filter(i => !i.defaultGameListID)" ><a class="dropdown-item" href="#" :class="{ 'active' : item.gameVMs.filter(h => h.id == game.id).length > 0 }">{{ item.name }}</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-else class="row">
                <div class="col-2">
                    <div class="bg-light d-flex" style="height: 100%;">
                        <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px; padding-top: 50%; padding-bottom: 50%;"/>
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
        name: "UserGameList",
        props: {
            userid: String
        },
        data: function () {
            return {
                items: [],
                selectedItem: null,
                successToast: {},
                successMessage: '',
                errorMessages: [],
                errorToast: {}
            };
        },       
        watch: {},
        created: function () {
            this.loadData();
        },
        mounted: function() {
            var that = this;
            that.successToast = new Toast(that.$refs.successtoast);
            that.errorToast = new Toast(that.$refs.errortoast);
        },
        methods: {
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetUserGameLists', { params: { userID: this.userid } })
                    .then(res => {
                        that.items = res.data;

                        that.items.forEach(i => {
                            i.groupedGames = that.getGroupedGames(i.gameVMs);
                        });

                        that.selectedItem = that.items[0];
                        
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
            getGroupedGames: function (games) {
                var groupedGames = [];

                if (games.length > 0) {
                    groupedGames.push(games.slice(0, 5));
                    var remainingGames = games.slice(5);

                    for (var i = 0; i < remainingGames.length; i += 6) {
                        groupedGames.push(remainingGames.slice(i, i + 6));
                    }
                }

                return groupedGames;
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
                var el = e.target;
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
                            that.successMessage = "Successfully added " + game.name + " to " + userGameList.name;
                            that.successToast.show();
                            el.add('active');
                        } else {
                            that.errorMessages = res.data.errorMessages;
                            that.$nextTick(function() {
                                that.$refs.errortoasts?.forEach(el => {
                                    new Toast(el).show();
                                });
                            });
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },          
            removeGameFromUserGameList(el, userGameList, game) {
                var that = this;
                
                return axios.post('/User/RemoveGameFromUserGameList', null,{ params: { userGameListID: userGameList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            that.successMessage = "Successfully removed " + game.name + " from " + userGameList.name;
                            that.successToast.show();
                            el.remove('active');
                        } else {
                            that.errorMessages = res.data.errorMessages;
                            that.$nextTick(function() {
                                that.$refs.errortoasts?.forEach(el => {
                                    new Toast(el).show();
                                });
                            });
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }                
        },
    };
</script>






