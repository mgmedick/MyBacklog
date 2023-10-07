<template>
    <div>
        <div class="show-lg position-relative">
            <div class="position-absolute top-0 bottom-0 start-0 end-0 pe-3" style="width: 280px;">
                <ul class="nav nav-pills flex-column mb-auto">
                    <li class="nav-item">
                        <a @click="onUserListClick(0)" href="#/" class="nav-link text-dark" :class="{ 'active' : selectedItemID == 0 }">
                            <div class="row no-gutters">
                                <div class="col-2">
                                    <font-awesome-icon icon="fa-solid fa-layer-group" size="lg" class="me-3"/>
                                </div>
                                <div class="col">
                                    <span>All Games</span>
                                </div>
                            </div>
                        </a>
                    </li>                
                    <li v-for="(userList, userListIndex) in userlists" key="userList.id" class="nav-item">
                        <a @click="onUserListClick(userList.id)" href="#" class="nav-link text-dark" :class="{ 'active' : selectedItemID == userList.id }">
                            <div class="row no-gutters">
                                <div class="col-2">
                                    <font-awesome-icon v-if="userList.defaultListID" :icon="getDefaultIconClass(userList.defaultListID)" size="lg" class="me-3"/>
                                    <font-awesome-icon v-else-if="userList.accountTypeID" :icon="getAccountIconClass(userList.accountTypeID)" size="lg" class="me-3"/>
                                    <font-awesome-icon v-else icon="fa-solid fa-list" size="lg" class="me-3"/>
                                </div>
                                <div class="col">
                                    <span>{{ userList.name }}</span>
                                </div>
                            </div>
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="show-md row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn btn-lg dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="selectedItemID == 0" icon="fa-solid fa-layer-group" size="lg"/>
                    <font-awesome-icon v-else-if="userlists.find(i => i.id == selectedItemID)?.defaultListID" :icon="getDefaultIconClass(userlists.find(i => i.id == selectedItemID)?.defaultListID)" size="lg"/>
                    <font-awesome-icon v-else-if="userlists.find(i => i.id == selectedItemID)?.accountTypeID" :icon="getAccountIconClass(userlists.find(i => i.id == selectedItemID)?.accountTypeID)" size="lg"/>
                    <font-awesome-icon v-else icon="fa-solid fa-list" size="lg"/>
                    <span class="mx-auto">{{ selectedItemID == 0 ? 'All Games' : userlists.find(i => i.id == selectedItemID)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li>
                        <a @click="onUserListClick(0)" class="dropdown-item" :class="{ 'active' : selectedItemID == 0 }" href="#/" data-toggle="pill">All Games</a>
                    </li>   
                    <li v-for="(userList, userListIndex) in userlists" :key="userList.id">
                        <a @click="onUserListClick(userList.id)" class="dropdown-item" :class="{ 'active' : selectedItemID == userList.id }" href="#/" data-toggle="pill">{{ userList.name }}</a>
                    </li>                 
                </ul>
            </div>                
        </div>
        <user-list-games ref="userlistgames" :userlists="userlists" :userlistid="selectedItemID" :emptycoverimagepath="emptycoverimagepath" :showimport="showimport"></user-list-games>           
    </div>
</template>
<script>
    export default {
        name: "UserLists",
        props: {
            userlists: Array,
            emptycoverimagepath: String,
            showimport: Boolean
        },
        data: function () {
            return {
                selectedItemID: sessionStorage.getItem('selectedUserListID') ?? this.userlists[0].id,
                successMessages: [],
                errorMessages: []
            };
        },       
        watch: {},
        created: function () {
            var that = this;
        },
        mounted: function() {
            var that = this;      
        },
        methods: {
            onUserListClick(userListID) {
                this.selectedItemID = userListID;
                sessionStorage.setItem('selectedUserListID', userListID.toString());
            },
            getDefaultIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 3:
                        iconClass = 'fa-solid fa-check';
                        break;                                         
                }

                return iconClass;
            },   
            getAccountIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-brands fa-steam';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox';
                        break;                                    
                }

                return iconClass;
            },                                                                         
        },
    };
</script>






