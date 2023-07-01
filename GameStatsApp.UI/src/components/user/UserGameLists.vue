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
                <li v-for="(userGameList, userGameListIndex) in userGameLists.filter(i => i.defaultGameListID)" key="userGameList.id" class="nav-item">
                    <a @click="selectedItemID = userGameList.id" href="#" class="nav-link" :class="{ 'active' : selectedItemID == userGameList.id }">
                        <font-awesome-icon :icon="getIconClass(userGameList.defaultGameListID)" size="lg" class="me-3"/>
                        <span>{{ userGameList.name }}</span>
                    </a>
                </li>
                <li v-if="userGameLists.filter(i => !i.defaultGameListID).length > 0" class="border-top my-3"></li>
                <li v-for="(userGameList, userGameListIndex) in userGameLists.filter(i => !i.defaultGameListID)" key="userGameList.id" class="nav-item">
                    <a @click="selectedItemID = userGameList.id" href="#" class="nav-link" :class="{ 'active' : selectedItemID == userGameList.id }">
                        <span>{{ userGameList.name }}</span>
                    </a>
                </li>                
            </ul>
        </div>
        <div class="show-sm row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="userGameLists.find(i => i.id == selectedItemID)?.defaultGameListID" :icon="getIconClass(userGameLists.find(i => i.id == selectedItemID)?.defaultGameListID)" size="lg"/>
                    <span class="mx-auto">{{ userGameLists.find(i => i.id == selectedItemID)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li v-for="(userGameList, userGameListIndex) in userGameLists">
                        <a :key="userGameList.id" @click="selectedItemID = userGameList.id" class="dropdown-item" :class="{ 'active' : selectedItemID == userGameList.id }" href="#/" data-toggle="pill">{{ userGameList.name }}</a>
                    </li>
                </ul>
            </div>                
        </div>
        <user-gamelist-games :userid="userid" :usergamelistid="selectedItemID" :usergamelists="userGameLists" @success="onSuccess" @error="onError"></user-gamelist-games>
    </div>
</template>
<script>
    import axios from 'axios';
    import { Toast } from 'bootstrap';

    export default {
        name: "UserGameLists",
        props: {
            userid: String
        },
        data: function () {
            return {
                userGameLists: [],
                selectedItemID: 0,
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
                        that.userGameLists = res.data;
                        that.selectedItemID = that.userGameLists[0].id;
                       
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
            onSuccess(successMsg) {
                var that = this;

                that.successMessage = successMsg;
                that.successToast.show();
            },
            onError(errorMsgs) {
                var that = this;
                
                that.errorMessages = errorMsgs;
                that.$nextTick(function() {
                    that.$refs.errortoasts?.forEach(el => {
                        new Toast(el).show();
                    });
                }); 
            },                         
        },
    };
</script>






