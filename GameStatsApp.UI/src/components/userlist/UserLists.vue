﻿<template>
    <div>
        <div class="show-lg position-relative">
            <div class="position-absolute top-0 bottom-0 start-0 end-0 pe-3" style="width: 280px;">
                <ul class="nav nav-pills flex-column mb-auto">
                    <li class="nav-item">
                        <a @click="onUserListClick(0)" href="#/" class="nav-link text-dark"
                            :class="{ 'active': selectedItemID == 0 }">
                            <div class="row no-gutters">
                                <div class="col-2">
                                    <font-awesome-icon icon="fa-solid fa-layer-group" size="lg" class="me-3" />
                                </div>
                                <div class="col">
                                    <span>All</span>
                                </div>
                            </div>
                        </a>
                    </li>
                    <li v-for="(userList, userListIndex) in userlists" key="userList.id" class="nav-item">
                        <a @click="onUserListClick(userList.id)" href="#" class="nav-link text-dark"
                            :class="{ 'active': selectedItemID == userList.id }">
                            <div class="row no-gutters">
                                <div class="col-2">
                                    <font-awesome-icon v-if="userList.defaultListID"
                                        :icon="getDefaultIconClass(userList.defaultListID)" size="lg" class="me-3" />
                                    <font-awesome-icon v-else icon="fa-solid fa-list" size="lg" class="me-3" />
                                </div>
                                <div class="col">
                                    <span>{{ userList.name }}</span>
                                </div>
                            </div>
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="/UserSettings/#managelists" class="nav-link">
                            <div class="row no-gutters">
                                <div class="col-2">
                                    <font-awesome-icon icon="fa-solid fa-gear" size="lg" class="me-3" />
                                </div>
                                <div class="col">
                                    <span>Manage lists</span>
                                </div>
                            </div>
                        </a>
                    </li>
                </ul>
            </div>
        </div>
        <div class="show-md row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button"
                    data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="selectedItemID == 0" icon="fa-solid fa-layer-group" size="xl" />
                    <font-awesome-icon v-else-if="userlists.find(i => i.id == selectedItemID)?.defaultListID"
                        :icon="getDefaultIconClass(userlists.find(i => i.id == selectedItemID)?.defaultListID)" size="xl" />
                    <font-awesome-icon v-else icon="fa-solid fa-list" size="xl" />
                    <span class="mx-auto fs-6">{{ selectedItemID == 0 ? 'All' : userlists.find(i => i.id ==
                        selectedItemID)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li>
                        <a @click="onUserListClick(0)" class="dropdown-item" :class="{ 'active': selectedItemID == 0 }"
                            href="#/" data-toggle="pill">All</a>
                    </li>
                    <li v-for="(userList, userListIndex) in userlists" :key="userList.id">
                        <a @click="onUserListClick(userList.id)" class="dropdown-item"
                            :class="{ 'active': selectedItemID == userList.id }" href="#/" data-toggle="pill">{{
                                userList.name }}</a>
                    </li>
                    <li>
                        <hr class="dropdown-divider">
                    </li>
                    <li class="nav">
                        <div class="nav-item">
                            <a href="/UserSettings/#managelists" class="nav-link" data-toggle="pill"
                                style="padding: var(--bs-dropdown-item-padding-y) var(--bs-dropdown-item-padding-x)">Manage lists</a>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
        <user-list-games ref="userlistgames" :userlists="userlists" :userlistid="selectedItemID"
            :showimport="showimport"></user-list-games>
    </div>
</template>
<script>
export default {
    name: "UserLists",
    props: {
        userlists: Array,
        showimport: Boolean
    },
    data: function () {
        return {
            selectedItemID: sessionStorage.getItem('selectedUserListID') ?? this.userlists[0].id,
            userList: {},
            successMessages: [],
            errorMessages: []
        };
    },
    watch: {},
    created: function () {
        var that = this;
    },
    mounted: function () {
        var that = this;
    },
    methods: {
        onEditListSaved(result) {
            Modal.getInstance(this.$refs.editlistmodal).hide();
            this.loadData();
        },
        onAddListClick() {
            this.userList = { id: 0, name: '', active: true, sortOrder: 0 };
            new Modal(this.$refs.editlistmodal, { backdrop: 'static' }).show();
        },
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
    },
};
</script>






